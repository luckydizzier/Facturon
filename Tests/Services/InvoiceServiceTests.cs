using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Repositories;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.Services
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _invoiceRepo = new();
        private readonly Mock<ISupplierRepository> _supplierRepo = new();
        private readonly Mock<IPaymentMethodRepository> _paymentRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<ITaxRateRepository> _taxRepo = new();

        private InvoiceService CreateService() => new(
            _invoiceRepo.Object,
            _supplierRepo.Object,
            _paymentRepo.Object,
            _productRepo.Object,
            _taxRepo.Object);

        [Fact]
        public async Task CreateAsync_ReturnsOk_WhenInvoiceValid()
        {
            var invoice = new Invoice
            {
                Date = DateTime.UtcNow,
                Number = "INV001",
                Issuer = "Acme",
                SupplierId = 1,
                PaymentMethodId = 2,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ProductId = 3, Quantity = 2, UnitPrice = 5 }
                }
            };

            var tax = new TaxRate { Id = 5, Code = "T", Value = 15m };
            _supplierRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Supplier { Active = true });
            _paymentRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new PaymentMethod { Active = true });
            _productRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Product { Active = true, TaxRateId = 5 });
            _taxRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(tax);

            Invoice? saved = null;
            _invoiceRepo.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .Callback<Invoice>(i => saved = i)
                .Returns(Task.CompletedTask);

            var service = CreateService();
            var result = await service.CreateAsync(invoice);

            Assert.True(result.Success);
            _invoiceRepo.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
            Assert.NotNull(saved);
            var savedItem = Assert.Single(saved!.Items);
            Assert.Equal(5, savedItem.TaxRateId);
            Assert.Equal(15m, savedItem.TaxRateValue);
        }

        [Fact]
        public async Task CreateAsync_ReturnsFail_WhenValidationFails()
        {
            var invoice = new Invoice
            {
                Date = DateTime.UtcNow,
                Number = "INV001",
                Issuer = "Acme",
                SupplierId = 1,
                PaymentMethodId = 2,
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { ProductId = 3, Quantity = 2, UnitPrice = 5 }
                }
            };

            _supplierRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Supplier?)null);
            _paymentRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new PaymentMethod { Active = true });
            _productRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(new Product { Active = true });

            var service = CreateService();
            var result = await service.CreateAsync(invoice);

            Assert.False(result.Success);
            _invoiceRepo.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Never);
        }

        [Fact]
        public async Task CalculateTotalsAsync_ComputesTotalsCorrectly()
        {
            var tax = new TaxRate { Id = 1, Code = "A", Value = 10 };
            var product = new Product { Id = 1, Active = true, TaxRate = tax, TaxRateId = 1 };

            var invoice = new Invoice
            {
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem { Quantity = 2, UnitPrice = 10, Product = product, TaxRateId = 1, TaxRate = tax, TaxRateValue = 10 },
                    new InvoiceItem { Quantity = 1, UnitPrice = 20, Product = product, TaxRateId = 1, TaxRate = tax, TaxRateValue = 10 }
                }
            };

            var service = CreateService();
            var totals = await service.CalculateTotalsAsync(invoice);

            Assert.Equal(40m, totals.TotalNet);
            Assert.Equal(4m, totals.TotalVat);
            Assert.Equal(44m, totals.TotalGross);
            Assert.Single(totals.ByTaxRate);
            Assert.Equal("A", totals.ByTaxRate[0].TaxCode);
        }
    }
}
