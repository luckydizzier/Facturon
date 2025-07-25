using Facturon.App.ViewModels;
using Facturon.Domain.Entities;
using Facturon.Services;
using Moq;
using Xunit;

namespace Facturon.Tests.ViewModels
{
    public class InvoiceItemInputViewModelTests
    {
        private static InvoiceItemInputViewModel CreateVm()
        {
            var productService = new Mock<IProductService>();
            var unitService = new Mock<IUnitService>();
            var taxRateService = new Mock<ITaxRateService>();
            var confirmation = new Mock<IConfirmationDialogService>();
            var productDialog = new Mock<INewEntityDialogService<Product>>();
            var unitDialog = new Mock<INewEntityDialogService<Unit>>();
            var taxDialog = new Mock<INewEntityDialogService<TaxRate>>();
            var nav = new Mock<INavigationService>();
            return new InvoiceItemInputViewModel(
                productService.Object,
                unitService.Object,
                taxRateService.Object,
                confirmation.Object,
                productDialog.Object,
                unitDialog.Object,
                taxDialog.Object,
                nav.Object);
        }

        [Fact]
        public void Add_RaisesFocusRequested()
        {
            var vm = CreateVm();
            vm.ProductSelector.SelectedItem = new Product { Id = 1 };
            vm.UnitSelector.SelectedItem = new Unit { Id = 1 };
            vm.TaxRateSelector.SelectedItem = new TaxRate { Id = 1, Value = 20m };
            vm.Quantity = 1;
            vm.NetUnitPrice = 10m;

            var raised = false;
            vm.FocusRequested += () => raised = true;
            vm.AddCommand.Execute(null);

            Assert.True(raised);
            Assert.Equal(1m, vm.Quantity);
            Assert.Equal(0m, vm.NetUnitPrice);
        }
    }
}
