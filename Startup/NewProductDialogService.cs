using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels;
using Facturon.App.Views;

namespace Facturon.App
{
    public class NewProductDialogService : INewEntityDialogService<Product>
    {
        private readonly IUnitService _unitService;
        private readonly ITaxRateService _taxRateService;
        private readonly IProductGroupService _productGroupService;
        private readonly IProductService _productService;

        public NewProductDialogService(
            IUnitService unitService,
            ITaxRateService taxRateService,
            IProductGroupService productGroupService,
            IProductService productService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
        }

        public Product? ShowDialog()
        {
            var dialog = new NewProductDialog();
            var vm = new NewProductDialogViewModel(
                _unitService,
                _taxRateService,
                _productGroupService,
                _productService);
            vm.Initialize();
            dialog.DataContext = vm;
            vm.CloseRequested += p => dialog.DialogResult = p != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Product : null;
        }
    }
}
