using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public EditableComboWithAddViewModel<Unit> UnitSelector { get; }
        public EditableComboWithAddViewModel<TaxRate> TaxRateSelector { get; }
        public ObservableCollection<ProductGroup> ProductGroups { get; } = new();

        public Product Product { get; } = new Product
        {
            Name = string.Empty,
            Unit = null!,
            ProductGroup = null!,
            TaxRate = null!,
            NetUnitPrice = 0m
        };



        private ProductGroup? _selectedProductGroup;
        public ProductGroup? SelectedProductGroup
        {
            get => _selectedProductGroup;
            set
            {
                if (_selectedProductGroup != value)
                {
                    _selectedProductGroup = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        Product.ProductGroup = value;
                        Product.ProductGroupId = value.Id;
                    }
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

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

        public event Action<Product?>? CloseRequested;

        public NewProductDialogViewModel(
            IUnitService unitService,
            ITaxRateService taxRateService,
            IProductGroupService productGroupService,
            IProductService productService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxRateDialogService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
            _confirmationService = confirmationService;
            _unitDialogService = unitDialogService;
            _taxRateDialogService = taxRateDialogService;

            UnitSelector = new EditableComboWithAddViewModel<Unit>(_unitService, _confirmationService, _unitDialogService);
            UnitSelector.PropertyChanged += UnitSelectorOnPropertyChanged;
            TaxRateSelector = new EditableComboWithAddViewModel<TaxRate>(_taxRateService, _confirmationService, _taxRateDialogService);
            TaxRateSelector.PropertyChanged += TaxRateSelectorOnPropertyChanged;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
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

        public void Initialize()
        {
            UnitSelector.InitializeAsync().GetAwaiter().GetResult();
            TaxRateSelector.InitializeAsync().GetAwaiter().GetResult();

            ProductGroups.Clear();
            foreach (var g in _productGroupService.GetAllAsync().GetAwaiter().GetResult())
                ProductGroups.Add(g);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Product.Name)
                && UnitSelector.SelectedItem != null
                && TaxRateSelector.SelectedItem != null
                && SelectedProductGroup != null
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
            var result = await _productService.CreateAsync(Product);
            if (result.Success)
            {
                CloseRequested?.Invoke(Product);
            }
        }
    }
}
