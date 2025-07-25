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
        private readonly INewEntityDialogService<TaxRate> _taxRateDialogService;
        private readonly INewEntityDialogService<ProductGroup> _productGroupDialogService;

        public NewProductDialogService(
            IUnitService unitService,
            ITaxRateService taxRateService,
            IProductGroupService productGroupService,
            IProductService productService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxRateDialogService,
            INewEntityDialogService<ProductGroup> productGroupDialogService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
            _confirmationService = confirmationService;
            _unitDialogService = unitDialogService;
            _taxRateDialogService = taxRateDialogService;
            _productGroupDialogService = productGroupDialogService;
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
                _unitDialogService,
                _taxRateDialogService,
                _productGroupDialogService);
            dialog.DataContext = vm;
            vm.CloseRequested += p => dialog.DialogResult = p != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Product : null;
        }
    }
}
