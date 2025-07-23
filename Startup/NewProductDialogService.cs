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
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;

        public NewProductDialogService(
            IUnitService unitService,
            ITaxRateService taxRateService,
            IProductGroupService productGroupService,
            IProductService productService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Unit> unitDialogService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
            _confirmationService = confirmationService;
            _unitDialogService = unitDialogService;
        }

        public Product? ShowDialog()
        {
            var dialog = new NewProductDialog();
            var vm = new NewProductDialogViewModel(
                _unitService,
                _taxRateService,
                _productGroupService,
                _productService,
                _confirmationService,
                _unitDialogService);
            vm.Initialize();
            dialog.DataContext = vm;
            vm.CloseRequested += p => dialog.DialogResult = p != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Product : null;
        }
    }
}
