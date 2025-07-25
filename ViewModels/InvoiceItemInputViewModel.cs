using System;
using System.ComponentModel;
using System.Threading.Tasks;
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
        private readonly INavigationService _navigationService;

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
        public RelayCommand LoadCommand { get; }
        public RelayCommand MoveNextCommand { get; }
        public RelayCommand MovePreviousCommand { get; }

        public event Action<InvoiceItem>? ItemReadyToAdd;

        public InvoiceItemViewModel? EditingItem { get; private set; }
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            private set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public InvoiceItemInputViewModel(
            IProductService productService,
            IUnitService unitService,
            ITaxRateService taxRateService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Product> productDialogService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxDialogService,
            INavigationService navigationService)
        {
            _productService = productService;
            _unitService = unitService;
            _taxRateService = taxRateService;
            _confirmationService = confirmationService;
            _productDialogService = productDialogService;
            _unitDialogService = unitDialogService;
            _taxDialogService = taxDialogService;
            _navigationService = navigationService;

            ProductSelector = new ProductSelectorViewModel(_productService, _confirmationService, _productDialogService);
            ProductSelector.PropertyChanged += ProductSelectorOnPropertyChanged;
            UnitSelector = new UnitSelectorViewModel(_unitService, _confirmationService, _unitDialogService);
            UnitSelector.PropertyChanged += SelectorOnPropertyChanged;
            TaxRateSelector = new TaxRateSelectorViewModel(_taxRateService, _confirmationService, _taxDialogService);
            TaxRateSelector.PropertyChanged += SelectorOnPropertyChanged;

            AddCommand = new RelayCommand(Add, IsValid);
            ClearCommand = new RelayCommand(Clear);
            LoadCommand = new RelayCommand(async () => await InitializeAsync());
            MoveNextCommand = new RelayCommand(() => _navigationService.MoveNext());
            MovePreviousCommand = new RelayCommand(() => _navigationService.MovePrevious());
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

        public async Task InitializeAsync()
        {
            await ProductSelector.InitializeAsync();
            await UnitSelector.InitializeAsync();
            await TaxRateSelector.InitializeAsync(DateTime.Today);
        }

        private void Add()
        {
            if (!IsValid())
                return;

            if (IsEditing)
            {
                CommitEdit();
                return;
            }

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

        public void BeginEdit(InvoiceItemViewModel item)
        {
            EditingItem = item;
            IsEditing = true;

            ProductSelector.SelectedItem = item.Product;
            ProductSelector.Input = item.Product.Name;
            if (item.Product.Unit != null)
            {
                UnitSelector.SelectedItem = item.Product.Unit;
                UnitSelector.Input = item.Product.Unit.Name;
            }
            TaxRateSelector.SelectedItem = item.Item.TaxRate;
            TaxRateSelector.Input = item.Item.TaxRate.Code;
            Quantity = item.Quantity;
            NetUnitPrice = item.UnitPrice;
        }

        public void CommitEdit()
        {
            if (EditingItem == null)
                return;

            var product = ProductSelector.SelectedItem;
            var tax = TaxRateSelector.SelectedItem;
            if (product == null || tax == null)
                return;

            var invoiceItem = EditingItem.Item;
            invoiceItem.ProductId = product.Id;
            invoiceItem.Product = product;
            invoiceItem.Quantity = Quantity;
            invoiceItem.UnitPrice = NetUnitPrice;
            invoiceItem.TaxRateId = tax.Id;
            invoiceItem.TaxRate = tax;
            invoiceItem.TaxRateValue = tax.Value;

            EditingItem.RecalculateAmounts(invoiceItem.Invoice?.IsGrossBased ?? false);

            EditingItem = null;
            IsEditing = false;
            Clear();
        }
    }
}
