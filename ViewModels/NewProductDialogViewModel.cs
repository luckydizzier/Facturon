using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class NewProductDialogViewModel : BaseViewModel
    {
        private readonly IUnitService _unitService;
        private readonly ITaxRateService _taxRateService;
        private readonly IProductGroupService _productGroupService;
        private readonly IProductService _productService;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;
        private readonly INewEntityDialogService<TaxRate> _taxRateDialogService;
        private readonly INewEntityDialogService<ProductGroup> _productGroupDialogService;
        private readonly INavigationService _navigationService;

        public EditableComboWithAddViewModel<Unit> UnitSelector { get; }
        public TaxRateSelectorViewModel TaxRateSelector { get; }
        public ProductGroupSelectorViewModel ProductGroupSelector { get; }

        public Product Product { get; } = new Product
        {
            Name = string.Empty,
            Unit = null!,
            ProductGroup = null!,
            TaxRate = null!,
            NetUnitPrice = 0m
        };




        public decimal NetUnitPrice
        {
            get => Product.NetUnitPrice;
            set
            {
                if (Product.NetUnitPrice != value)
                {
                    Product.NetUnitPrice = value;
                    OnPropertyChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand LoadCommand { get; }
        public RelayCommand MoveNextCommand { get; }
        public RelayCommand MovePreviousCommand { get; }

        public event Action<Product?>? CloseRequested;

        public NewProductDialogViewModel(
            IUnitService unitService,
            ITaxRateService taxRateService,
            IProductGroupService productGroupService,
            IProductService productService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxRateDialogService,
            INewEntityDialogService<ProductGroup> productGroupDialogService,
            INavigationService navigationService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
            _confirmationService = confirmationService;
            _unitDialogService = unitDialogService;
            _taxRateDialogService = taxRateDialogService;
            _productGroupDialogService = productGroupDialogService;
            _navigationService = navigationService;

            UnitSelector = new EditableComboWithAddViewModel<Unit>(_unitService, _confirmationService, _unitDialogService);
            UnitSelector.PropertyChanged += UnitSelectorOnPropertyChanged;
            TaxRateSelector = new TaxRateSelectorViewModel(_taxRateService, _confirmationService, _taxRateDialogService);
            TaxRateSelector.PropertyChanged += TaxRateSelectorOnPropertyChanged;
            ProductGroupSelector = new ProductGroupSelectorViewModel(_productGroupService, _confirmationService, _productGroupDialogService);
            ProductGroupSelector.PropertyChanged += ProductGroupSelectorOnPropertyChanged;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
            LoadCommand = new RelayCommand(async () => await InitializeAsync());
            MoveNextCommand = new RelayCommand(() => _navigationService.MoveNext());
            MovePreviousCommand = new RelayCommand(() => _navigationService.MovePrevious());
        }

        private void UnitSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UnitSelector.SelectedItem))
                SaveCommand.RaiseCanExecuteChanged();
        }

        private void TaxRateSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaxRateSelector.SelectedItem))
            {
                if (TaxRateSelector.SelectedItem != null)
                {
                    Product.TaxRate = TaxRateSelector.SelectedItem;
                    Product.TaxRateId = TaxRateSelector.SelectedItem.Id;
                }
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private void ProductGroupSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductGroupSelector.SelectedItem))
            {
                if (ProductGroupSelector.SelectedItem != null)
                {
                    Product.ProductGroup = ProductGroupSelector.SelectedItem;
                    Product.ProductGroupId = ProductGroupSelector.SelectedItem.Id;
                }
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public async Task InitializeAsync()
        {
            await UnitSelector.InitializeAsync();
            await TaxRateSelector.InitializeAsync(DateTime.Today);
            await ProductGroupSelector.InitializeAsync();
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Product.Name)
                && UnitSelector.SelectedItem != null
                && TaxRateSelector.SelectedItem != null
                && ProductGroupSelector.SelectedItem != null
                && NetUnitPrice > 0m;
        }

        private async void Save()
        {
            var unit = UnitSelector.SelectedItem!;
            Product.Unit = unit;
            Product.UnitId = unit.Id;
            var rate = TaxRateSelector.SelectedItem!;
            Product.TaxRate = rate;
            Product.TaxRateId = rate.Id;
            var group = ProductGroupSelector.SelectedItem!;
            Product.ProductGroup = group;
            Product.ProductGroupId = group.Id;
            var result = await _productService.CreateAsync(Product);
            if (result.Success)
            {
                CloseRequested?.Invoke(Product);
            }
        }
    }
}
