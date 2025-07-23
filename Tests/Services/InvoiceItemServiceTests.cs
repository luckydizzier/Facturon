using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.Services
{
    public class InvoiceItemServiceTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<ITaxRateRepository> _taxRepo = new();

        private InvoiceItemService CreateService() => new(_invoiceRepo.Object, _productRepo.Object, _taxRepo.Object);

        [Fact]
        public async Task CreateAsync_StoresGrossAndNetTotal_WhenInvoiceIsGrossBased()
        {
            var invoice = new Invoice { Id = 1, Active = true, IsGrossBased = true, Items = new List<InvoiceItem>() };
            var product = new Product { Id = 2, Active = true, TaxRateId = 3 };
            var rate = new TaxRate { Id = 3, Value = 20m };
            var item = new InvoiceItem { InvoiceId = 1, ProductId = 2, Quantity = 1, UnitPrice = 120m };

            _invoiceRepo.Setup(r => r.AsQueryable()).Returns(new List<Invoice> { invoice }.AsQueryable());
            _productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product);
            _taxRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(rate);
            _invoiceRepo.Setup(r => r.UpdateAsync(invoice)).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.CreateAsync(item);

            var saved = Assert.Single(invoice.Items);
            Assert.Equal(120m, saved.UnitPrice);
            Assert.Equal(100m, saved.Total);
        }

        [Fact]
        public async Task CreateAsync_KeepsNetPrice_WhenInvoiceIsNetBased()
        {
            var invoice = new Invoice { Id = 1, Active = true, IsGrossBased = false, Items = new List<InvoiceItem>() };
            var product = new Product { Id = 2, Active = true, TaxRateId = 3 };
            var rate = new TaxRate { Id = 3, Value = 20m };
            var item = new InvoiceItem { InvoiceId = 1, ProductId = 2, Quantity = 2, UnitPrice = 50m };

            _invoiceRepo.Setup(r => r.AsQueryable()).Returns(new List<Invoice> { invoice }.AsQueryable());
            _productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product);
            _taxRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(rate);
            _invoiceRepo.Setup(r => r.UpdateAsync(invoice)).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.CreateAsync(item);

            var saved = Assert.Single(invoice.Items);
            Assert.Equal(50m, saved.UnitPrice);
            Assert.Equal(100m, saved.Total);
        }

        [Fact]
        public async Task UpdateAsync_RecalculatesTotal_WhenInvoiceIsGrossBased()
        {
            var invoice = new Invoice { Id = 1, Active = true, IsGrossBased = true, Items = new List<InvoiceItem>() };
            var existing = new InvoiceItem
            {
                Id = 10,
                ProductId = 2,
                Quantity = 1,
                UnitPrice = 120m,
                TaxRateId = 3,
                TaxRateValue = 20m,
                Product = new Product { Id = 2, Active = true, TaxRateId = 3 },
                TaxRate = new TaxRate { Id = 3, Value = 20m },
                Invoice = invoice
            };
            invoice.Items.Add(existing);
            var product = new Product { Id = 5, Active = true, TaxRateId = 3 };
            var rate = new TaxRate { Id = 3, Value = 20m };
            var update = new InvoiceItem { Id = 10, InvoiceId = 1, ProductId = 5, Quantity = 2, UnitPrice = 240m };

            _invoiceRepo.Setup(r => r.AsQueryable()).Returns(new List<Invoice> { invoice }.AsQueryable());
            _productRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(product);
            _taxRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(rate);
            _invoiceRepo.Setup(r => r.UpdateAsync(invoice)).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.UpdateAsync(update);

            Assert.Equal(5, existing.ProductId);
            Assert.Equal(240m, existing.UnitPrice);
            Assert.Equal(400m, existing.Total);
        }

        [Fact]
        public async Task UpdateAsync_RecalculatesTotal_WhenInvoiceIsNetBased()
        {
            var invoice = new Invoice { Id = 1, Active = true, IsGrossBased = false, Items = new List<InvoiceItem>() };
            var existing = new InvoiceItem
            {
                Id = 11,
                ProductId = 2,
                Quantity = 2,
                UnitPrice = 50m,
                TaxRateId = 3,
                TaxRateValue = 20m,
                Product = new Product { Id = 2, Active = true, TaxRateId = 3 },
                TaxRate = new TaxRate { Id = 3, Value = 20m },
                Invoice = invoice
            };
            invoice.Items.Add(existing);
            var product = new Product { Id = 2, Active = true, TaxRateId = 3 };
            var rate = new TaxRate { Id = 3, Value = 20m };
            var update = new InvoiceItem { Id = 11, InvoiceId = 1, ProductId = 2, Quantity = 3, UnitPrice = 50m };

            _invoiceRepo.Setup(r => r.AsQueryable()).Returns(new List<Invoice> { invoice }.AsQueryable());
            _productRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product);
            _taxRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(rate);
            _invoiceRepo.Setup(r => r.UpdateAsync(invoice)).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.UpdateAsync(update);

            Assert.Equal(3, existing.Quantity);
            Assert.Equal(150m, existing.Total);
        }
    }
}
