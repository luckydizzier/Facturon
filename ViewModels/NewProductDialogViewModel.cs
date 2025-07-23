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

        public EditableComboWithAddViewModel<Unit> UnitSelector { get; }
        public ObservableCollection<TaxRate> TaxRates { get; } = new();
        public ObservableCollection<ProductGroup> ProductGroups { get; } = new();

        public Product Product { get; } = new Product
        {
            Name = string.Empty,
            Unit = null!,
            ProductGroup = null!,
            TaxRate = null!,
            NetUnitPrice = 0m
        };


        private TaxRate? _selectedTaxRate;
        public TaxRate? SelectedTaxRate
        {
            get => _selectedTaxRate;
            set
            {
                if (_selectedTaxRate != value)
                {
                    _selectedTaxRate = value;
                    OnPropertyChanged();
                    if (value != null)
                    {
                        Product.TaxRate = value;
                        Product.TaxRateId = value.Id;
                    }
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

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
            INewEntityDialogService<Unit> unitDialogService)
        {
            _unitService = unitService;
            _taxRateService = taxRateService;
            _productGroupService = productGroupService;
            _productService = productService;
            _confirmationService = confirmationService;
            _unitDialogService = unitDialogService;

            UnitSelector = new EditableComboWithAddViewModel<Unit>(_unitService, _confirmationService, _unitDialogService);
            UnitSelector.PropertyChanged += UnitSelectorOnPropertyChanged;

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private void UnitSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UnitSelector.SelectedItem))
                SaveCommand.RaiseCanExecuteChanged();
        }

        public void Initialize()
        {
            UnitSelector.InitializeAsync().GetAwaiter().GetResult();

            TaxRates.Clear();
            foreach (var t in _taxRateService.GetAllAsync().GetAwaiter().GetResult())
                TaxRates.Add(t);

            ProductGroups.Clear();
            foreach (var g in _productGroupService.GetAllAsync().GetAwaiter().GetResult())
                ProductGroups.Add(g);
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Product.Name)
                && UnitSelector.SelectedItem != null
                && SelectedTaxRate != null
                && SelectedProductGroup != null
                && NetUnitPrice > 0m;
        }

        private async void Save()
        {
            var unit = UnitSelector.SelectedItem!;
            Product.Unit = unit;
            Product.UnitId = unit.Id;
            var result = await _productService.CreateAsync(Product);
            if (result.Success)
            {
                CloseRequested?.Invoke(Product);
            }
        }
    }
}
