using System;
using System.ComponentModel;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceItemInputViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        private readonly IUnitService _unitService;
        private readonly ITaxRateService _taxRateService;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<Product> _productDialogService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;
        private readonly INewEntityDialogService<TaxRate> _taxDialogService;

        public ProductSelectorViewModel ProductSelector { get; }
        public UnitSelectorViewModel UnitSelector { get; }
        public TaxRateSelectorViewModel TaxRateSelector { get; }

        private decimal _quantity = 1m;
        public decimal Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private decimal _netUnitPrice;
        public decimal NetUnitPrice
        {
            get => _netUnitPrice;
            set
            {
                if (_netUnitPrice != value)
                {
                    _netUnitPrice = value;
                    OnPropertyChanged();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand AddCommand { get; }
        public RelayCommand ClearCommand { get; }

        public event Action<InvoiceItem>? ItemReadyToAdd;

        public InvoiceItemInputViewModel(
            IProductService productService,
            IUnitService unitService,
            ITaxRateService taxRateService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Product> productDialogService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxDialogService)
        {
            _productService = productService;
            _unitService = unitService;
            _taxRateService = taxRateService;
            _confirmationService = confirmationService;
            _productDialogService = productDialogService;
            _unitDialogService = unitDialogService;
            _taxDialogService = taxDialogService;

            ProductSelector = new ProductSelectorViewModel(_productService, _confirmationService, _productDialogService);
            ProductSelector.PropertyChanged += ProductSelectorOnPropertyChanged;
            UnitSelector = new UnitSelectorViewModel(_unitService, _confirmationService, _unitDialogService);
            UnitSelector.PropertyChanged += SelectorOnPropertyChanged;
            TaxRateSelector = new TaxRateSelectorViewModel(_taxRateService, _confirmationService, _taxDialogService);
            TaxRateSelector.PropertyChanged += SelectorOnPropertyChanged;

            AddCommand = new RelayCommand(Add, IsValid);
            ClearCommand = new RelayCommand(Clear);
        }

        private void ProductSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductSelector.SelectedItem) && ProductSelector.SelectedItem != null)
            {
                PopulateFromProduct(ProductSelector.SelectedItem);
            }
        }

        private void SelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditableComboWithAddViewModel<object>.SelectedItem))
                AddCommand.RaiseCanExecuteChanged();
        }

        public void Initialize()
        {
            ProductSelector.InitializeAsync().GetAwaiter().GetResult();
            UnitSelector.InitializeAsync().GetAwaiter().GetResult();
            TaxRateSelector.InitializeAsync(DateTime.Today).GetAwaiter().GetResult();
        }

        private void Add()
        {
            if (!IsValid())
                return;

            var product = ProductSelector.SelectedItem!;
            var tax = TaxRateSelector.SelectedItem!;
            var item = new InvoiceItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = Quantity,
                UnitPrice = NetUnitPrice,
                TaxRateId = tax.Id,
                TaxRate = tax,
                TaxRateValue = tax.Value,
                Invoice = null!
            };

            ItemReadyToAdd?.Invoke(item);
            Clear();
        }

        private void Clear()
        {
            ProductSelector.SelectedItem = null;
            ProductSelector.Input = string.Empty;
            UnitSelector.SelectedItem = null;
            UnitSelector.Input = string.Empty;
            TaxRateSelector.SelectedItem = null;
            TaxRateSelector.Input = string.Empty;
            Quantity = 1m;
            NetUnitPrice = 0m;
        }

        public bool IsValid()
        {
            return ProductSelector.SelectedItem != null
                && UnitSelector.SelectedItem != null
                && TaxRateSelector.SelectedItem != null
                && Quantity > 0m
                && NetUnitPrice > 0m;
        }

        private void PopulateFromProduct(Product product)
        {
            if (product.Unit != null)
                UnitSelector.SelectedItem = product.Unit;
            if (product.TaxRate != null)
                TaxRateSelector.SelectedItem = product.TaxRate;
            NetUnitPrice = product.NetUnitPrice;
        }
    }
}
