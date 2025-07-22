using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Facturon.Domain.Entities;
using Facturon.Repositories;

namespace Facturon.Services
{
    public class InvoiceItemService : IInvoiceItemService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IProductRepository _productRepository;
        private readonly ITaxRateRepository _taxRateRepository;

        public InvoiceItemService(IInvoiceRepository invoiceRepository, IProductRepository productRepository, ITaxRateRepository taxRateRepository)
        {
            _invoiceRepository = invoiceRepository;
            _productRepository = productRepository;
            _taxRateRepository = taxRateRepository;
        }

        public async Task<InvoiceItem?> GetByIdAsync(int id)
        {
            return await _invoiceRepository.AsQueryable()
                .SelectMany(i => i.Items)
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.Id == id);
        }

        public async Task<List<InvoiceItem>> GetByInvoiceAsync(int invoiceId)
        {
            return await _invoiceRepository.AsQueryable()
                .Where(i => i.Id == invoiceId)
                .SelectMany(i => i.Items)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Result> CreateAsync(InvoiceItem item)
        {
            var invoice = await _invoiceRepository.AsQueryable()
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == item.InvoiceId);
            if (invoice == null || !invoice.Active)
                return Result.Fail("Invalid invoice");

            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null || !product.Active)
                return Result.Fail("Invalid product");
            var rate = product.TaxRate ?? await _taxRateRepository.GetByIdAsync(product.TaxRateId);
            var rate = product.TaxRate ?? await _taxRateRepository.GetByIdAsync(product.TaxRateId);

            if (item.Quantity <= 0)
                return Result.Fail("Quantity must be greater than zero");
            if (item.UnitPrice < 0)
                return Result.Fail("Unit price must be non-negative");

            item.TaxRateId = product.TaxRateId;
            item.TaxRate = rate!;
            item.TaxRateValue = rate?.Value ?? 0m;

            var unit = item.UnitPrice;
            if (invoice.IsGrossBased)
                unit = unit / (1 + item.TaxRateValue / 100m);
            item.UnitPrice = Math.Round(unit, 2);
            item.Total = Math.Round(item.Quantity * item.UnitPrice, 2);

            item.DateCreated = DateTime.UtcNow;
            item.DateUpdated = DateTime.UtcNow;
            item.Active = true;

            invoice.Items.Add(item);
            await _invoiceRepository.UpdateAsync(invoice);
            return Result.Ok();
        }

        public async Task<Result> UpdateAsync(InvoiceItem item)
        {
            var invoice = await _invoiceRepository.AsQueryable()
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Items.Any(it => it.Id == item.Id));
            if (invoice == null)
                return Result.Fail("Invoice item not found");

            var existing = invoice.Items.First(it => it.Id == item.Id);
            if (!existing.Active)
                return Result.Fail("Invoice item not found");

            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null || !product.Active)
                return Result.Fail("Invalid product");
            var rate = product.TaxRate ?? await _taxRateRepository.GetByIdAsync(product.TaxRateId);

            if (item.Quantity <= 0)
                return Result.Fail("Quantity must be greater than zero");
            if (item.UnitPrice < 0)
                return Result.Fail("Unit price must be non-negative");

            existing.ProductId = item.ProductId;
            existing.Quantity = item.Quantity;
            var unit = item.UnitPrice;
            if (invoice.IsGrossBased)
                unit = unit / (1 + existing.TaxRateValue / 100m);
            existing.UnitPrice = Math.Round(unit, 2);
            existing.Total = Math.Round(item.Quantity * existing.UnitPrice, 2);
            existing.TaxRateId = product.TaxRateId;
            existing.TaxRate = rate!;
            existing.TaxRateValue = rate?.Value ?? 0m;
            existing.DateUpdated = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var invoice = await _invoiceRepository.AsQueryable()
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Items.Any(it => it.Id == id));
            if (invoice == null)
                return Result.Fail("Invoice item not found");

            var item = invoice.Items.First(it => it.Id == id);
            if (!item.Active)
                return Result.Fail("Invoice item not found");

            item.Active = false;
            item.DateUpdated = DateTime.UtcNow;

            await _invoiceRepository.UpdateAsync(invoice);
            return Result.Ok();
        }
    }
}
