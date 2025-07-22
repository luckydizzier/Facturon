using Facturon.App.ViewModels;
using Facturon.Domain.Entities;
using Xunit;

namespace Facturon.Tests.ViewModels
{
    public class InvoiceItemViewModelTests
    {
        [Fact]
        public void RecalculateAmounts_ToGrossBased_ComputesValues()
        {
            var invoice = new Invoice { IsGrossBased = false };
            var item = new InvoiceItem
            {
                Quantity = 2,
                UnitPrice = 10m,
                TaxRateValue = 20m,
                Product = new Product(),
                TaxRate = new TaxRate(),
                Invoice = invoice
            };

            var vm = new InvoiceItemViewModel(item);
            vm.RecalculateAmounts(true);

            Assert.Equal(20m, vm.GrossAmount);
            Assert.Equal(16.67m, vm.NetAmount);
            Assert.Equal(3.33m, vm.TaxAmount);
        }

        [Fact]
        public void RecalculateAmounts_ToNetBased_ComputesValues()
        {
            var invoice = new Invoice { IsGrossBased = true };
            var item = new InvoiceItem
            {
                Quantity = 1,
                UnitPrice = 110m,
                TaxRateValue = 10m,
                Product = new Product(),
                TaxRate = new TaxRate(),
                Invoice = invoice
            };

            var vm = new InvoiceItemViewModel(item);
            vm.RecalculateAmounts(false);

            Assert.Equal(110m, vm.GrossAmount);
            Assert.Equal(100m, vm.NetAmount);
            Assert.Equal(10m, vm.TaxAmount);
        }
    }
}
