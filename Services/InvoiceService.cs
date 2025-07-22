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
            return await _invoiceRepository.GetByIdAsync(id);
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
                        result.AddError($"Items[{i}].ProductId", "Invalid product");
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

        public async Task<InvoiceTotals> CalculateTotalsAsync(Invoice invoice)
        {
            var totals = new InvoiceTotals();
            var groups = new Dictionary<int, TaxRateTotal>();

            foreach (var item in invoice.Items)
            {
                var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null) continue;
                var taxRate = product.TaxRate ?? await _taxRateRepository.GetByIdAsync(product.TaxRateId);
                if (taxRate == null) continue;

                var net = item.Quantity * item.UnitPrice;
                var vat = net * taxRate.Value / 100m;
                var gross = net + vat;

                if (!groups.TryGetValue(taxRate.Id, out var tg))
                {
                    tg = new TaxRateTotal { TaxCode = taxRate.Code };
                    groups[taxRate.Id] = tg;
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
