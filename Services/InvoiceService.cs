using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Facturon.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITaxRateRepository _taxRateRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            ISupplierRepository supplierRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IProductRepository productRepository,
            ITaxRateRepository taxRateRepository)
        {
            _invoiceRepository = invoiceRepository;
            _supplierRepository = supplierRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _productRepository = productRepository;
            _taxRateRepository = taxRateRepository;
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _invoiceRepository.AsQueryable()
                .Where(i => i.Id == id)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                        .ThenInclude(p => p.Unit)
                .Include(i => i.Items)
                    .ThenInclude(it => it.Product)
                        .ThenInclude(p => p.TaxRate)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _invoiceRepository.GetAllAsync();
        }

        public async Task<List<Invoice>> GetInvoicesAsync()
        {
            return await GetAllAsync();
        }

        private async Task<ValidationResult> ValidateInvoiceAsync(Invoice invoice)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(invoice.Number))
                result.AddError(nameof(invoice.Number), "Number is required");

            if (string.IsNullOrWhiteSpace(invoice.Issuer))
                result.AddError(nameof(invoice.Issuer), "Issuer is required");

            if (invoice.Date == default)
                result.AddError(nameof(invoice.Date), "Date is required");

            var supplier = await _supplierRepository.GetByIdAsync(invoice.SupplierId);
            if (supplier == null || !supplier.Active)
                result.AddError(nameof(invoice.SupplierId), "Invalid supplier");

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(invoice.PaymentMethodId);
            if (paymentMethod == null || !paymentMethod.Active)
                result.AddError(nameof(invoice.PaymentMethodId), "Invalid payment method");

            if (invoice.Items == null || invoice.Items.Count == 0)
            {
                result.AddError(nameof(invoice.Items), "At least one item is required");
            }
            else
            {
                for (int i = 0; i < invoice.Items.Count; i++)
                {
                    var item = invoice.Items[i];
                    if (item.Quantity <= 0)
                        result.AddError($"Items[{i}].Quantity", "Quantity must be greater than zero");
                    if (item.UnitPrice < 0)
                        result.AddError($"Items[{i}].UnitPrice", "Unit price must be non-negative");

                    var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null || !product.Active)
                    {
                        result.AddError($"Items[{i}].ProductId", "Invalid product");
                    }
                    else
                    {
                        var rate = product.TaxRate ?? await _taxRateRepository.GetByIdAsync(product.TaxRateId);
                        item.TaxRateId = product.TaxRateId;
                        item.TaxRate = rate!;
                        item.TaxRateValue = rate?.Value ?? 0m;
                    }
                }
            }

            return result;
        }

        public async Task<Result> CreateAsync(Invoice invoice)
        {
            var validation = await ValidateInvoiceAsync(invoice);
            if (!validation.IsValid)
                return Result.Fail("Validation failed");

            invoice.DateCreated = DateTime.UtcNow;
            invoice.DateUpdated = DateTime.UtcNow;
            invoice.Active = true;

            foreach (var item in invoice.Items)
            {
                var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId);
                var rate = product?.TaxRate ?? await _taxRateRepository.GetByIdAsync(product?.TaxRateId ?? 0);
                item.TaxRateId = product?.TaxRateId ?? 0;
                item.TaxRate = rate!;
                item.TaxRateValue = rate?.Value ?? 0m;
                item.DateCreated = DateTime.UtcNow;
                item.DateUpdated = DateTime.UtcNow;
                item.Active = true;
            }

            await _invoiceRepository.AddAsync(invoice);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(Invoice invoice)
        {
            var existing = await _invoiceRepository.GetByIdAsync(invoice.Id);
            if (existing == null || !existing.Active)
                return Result.Fail("Invoice not found");

            var supplier = await _supplierRepository.GetByIdAsync(invoice.SupplierId);
            if (supplier == null || !supplier.Active)
                return Result.Fail("Invalid supplier");

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(invoice.PaymentMethodId);
            if (paymentMethod == null || !paymentMethod.Active)
                return Result.Fail("Invalid payment method");

            invoice.DateCreated = existing.DateCreated;
            invoice.DateUpdated = DateTime.UtcNow;
            invoice.Active = existing.Active;

            await _invoiceRepository.UpdateAsync(invoice);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null || !invoice.Active)
                return Result.Fail("Invoice not found");

            await _invoiceRepository.DeleteAsync(id);
            return Result.Ok();
        }

        public async Task<InvoiceTotals> GetTotalsAsync(int invoiceId)
        {
            var invoice = await GetByIdAsync(invoiceId);
            if (invoice == null)
                return new InvoiceTotals();

            return await CalculateTotalsAsync(invoice);
        }

        public async Task<InvoiceTotals> CalculateTotalsAsync(Invoice invoice)
        {
            var totals = new InvoiceTotals();
            var groups = new Dictionary<string, TaxRateTotal>();

            foreach (var item in invoice.Items)
            {
                var taxRate = item.TaxRate ?? await _taxRateRepository.GetByIdAsync(item.TaxRateId);
                decimal net;
                decimal vat;
                decimal gross;

                if (invoice.IsGrossBased)
                {
                    gross = item.Quantity * item.UnitPrice;
                    net = gross / (1 + item.TaxRateValue / 100m);
                    vat = gross - net;
                }
                else
                {
                    net = item.Quantity * item.UnitPrice;
                    vat = net * item.TaxRateValue / 100m;
                    gross = net + vat;
                }

                net = Math.Round(net, 2);
                vat = Math.Round(vat, 2);
                gross = Math.Round(gross, 2);

                var codeKey = taxRate?.Code ?? string.Empty;
                if (!groups.TryGetValue(codeKey, out var tg))
                {
                    tg = new TaxRateTotal { TaxCode = codeKey };
                    groups[codeKey] = tg;
                    totals.ByTaxRate.Add(tg);
                }

                tg.Net += net;
                tg.Vat += vat;
                tg.Gross += gross;
                totals.TotalNet += net;
                totals.TotalVat += vat;
                totals.TotalGross += gross;
            }

            return totals;
        }
    }
}
