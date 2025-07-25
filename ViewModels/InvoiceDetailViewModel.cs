using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceDetailViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IProductService _productService;
        private readonly IUnitService _unitService;
        private readonly ITaxRateService _taxRateService;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<PaymentMethod> _paymentMethodDialogService;
        private readonly INewEntityDialogService<Product> _productDialogService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;
        private readonly INewEntityDialogService<TaxRate> _taxDialogService;
        private readonly ISupplierService _supplierService;
        private readonly INewEntityDialogService<Supplier> _supplierDialogService;
        private readonly IInvoiceItemService _invoiceItemService;
        private readonly INavigationService _navigationService;
        private readonly MainViewModel _mainViewModel;

        public SupplierSelectorViewModel SupplierSelector { get; }
        public PaymentMethodSelectorViewModel PaymentMethodSelector { get; }
        public InvoiceItemInputViewModel InputRow { get; }
        public InvoiceItemViewModel? SelectedInvoiceItem
        {
            get => _selectedInvoiceItem;
            set
            {
                if (_selectedInvoiceItem != value)
                {
                    _selectedInvoiceItem = value;
                    OnPropertyChanged();
                    DeleteSelectedItemCommand.RaiseCanExecuteChanged();
                    BeginEditItemCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private InvoiceItemViewModel? _selectedInvoiceItem;
        public RelayCommand DeleteSelectedItemCommand { get; }
        public RelayCommand BeginEditItemCommand { get; }
        public RelayCommand LoadCommand { get; }

        public InvoiceDetailViewModel(
            IInvoiceService invoiceService,
            IPaymentMethodService paymentMethodService,
            IProductService productService,
            IUnitService unitService,
            ITaxRateService taxRateService,
            ISupplierService supplierService,
            IInvoiceItemService invoiceItemService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<PaymentMethod> paymentMethodDialogService,
            INewEntityDialogService<Product> productDialogService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxDialogService,
            INewEntityDialogService<Supplier> supplierDialogService,
            INavigationService navigationService,
            MainViewModel mainViewModel)
        {
            _invoiceService = invoiceService;
            _paymentMethodService = paymentMethodService;
            _productService = productService;
            _unitService = unitService;
            _taxRateService = taxRateService;
            _invoiceItemService = invoiceItemService;
            _confirmationService = confirmationService;
            _paymentMethodDialogService = paymentMethodDialogService;
            _productDialogService = productDialogService;
            _unitDialogService = unitDialogService;
            _taxDialogService = taxDialogService;
            _supplierService = supplierService;
            _navigationService = navigationService;
            _supplierDialogService = supplierDialogService;
            _mainViewModel = mainViewModel;

            PaymentMethodSelector = new PaymentMethodSelectorViewModel(
                _paymentMethodService,
                _confirmationService,
                _paymentMethodDialogService);
            PaymentMethodSelector.PropertyChanged += PaymentMethodSelectorOnPropertyChanged;

            SupplierSelector = new SupplierSelectorViewModel(
                _supplierService,
                _confirmationService,
                _supplierDialogService);
            SupplierSelector.PropertyChanged += SupplierSelectorOnPropertyChanged;

            InputRow = new InvoiceItemInputViewModel(
                _productService,
                _unitService,
                _taxRateService,
                _confirmationService,
                _productDialogService,
                _unitDialogService,
                _taxDialogService,
                _navigationService);
            InputRow.ItemReadyToAdd += InputRowOnItemReadyToAdd;

            DeleteSelectedItemCommand = new RelayCommand(DeleteSelectedItem, CanDeleteSelectedItem);
            BeginEditItemCommand = new RelayCommand(BeginEditItem, CanBeginEditItem);
            LoadCommand = new RelayCommand(async () => await InitializeAsync());

            InvoiceItems = new ObservableCollection<InvoiceItemViewModel>();
            _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
        }

        public async Task InitializeAsync()
        {
            await PaymentMethodSelector.InitializeAsync();
            await SupplierSelector.InitializeAsync();
        }

        private Invoice? _invoice;
        public Invoice? Invoice
        {
            get => _invoice;
            set
            {
                if (_invoice != value)
                {
                    _invoice = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(InvoiceItems));
                    OnPropertyChanged(nameof(IsGrossBased));
                    PaymentMethodSelector.SelectedItem = _invoice?.PaymentMethod;
                    SupplierSelector.SelectedItem = _invoice?.Supplier;
                }
            }
        }

        public bool IsGrossBased
        {
            get => Invoice?.IsGrossBased ?? false;
            set
            {
                if (Invoice != null && Invoice.IsGrossBased != value)
                {
                    Invoice.IsGrossBased = value;
                    OnPropertyChanged();
                    foreach (var item in InvoiceItems)
                        item.RecalculateAmounts(value);
                    _ = RecalculateTotalsAsync();
                }
            }
        }

        private Visibility _detailVisible = Visibility.Collapsed;
        public Visibility DetailVisible
        {
            get => _detailVisible;
            set
            {
                if (_detailVisible != value)
                {
                    _detailVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<InvoiceItemViewModel> _invoiceItems = new();
        public ObservableCollection<InvoiceItemViewModel> InvoiceItems
        {
            get => _invoiceItems;
            private set
            {
                if (_invoiceItems != value)
                {
                    _invoiceItems = value;
                    OnPropertyChanged();
                }
            }
        }

        private InvoiceTotals _invoiceTotals = new InvoiceTotals();
        public InvoiceTotals InvoiceTotals
        {
            get => _invoiceTotals;
            private set
            {
                if (_invoiceTotals != value)
                {
                    _invoiceTotals = value;
                    OnPropertyChanged();
                }
            }
        }

        private async void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedInvoice))
            {
                await LoadInvoiceAsync();
                UpdateDetailVisibility();
            }
        }

        private async Task LoadInvoiceAsync()
        {
            if (_mainViewModel.SelectedInvoice == null)
            {
                Invoice = null;
                InvoiceItems.Clear();
                InvoiceTotals = new InvoiceTotals();
                return;
            }

            var full = await _invoiceService.GetByIdAsync(_mainViewModel.SelectedInvoice.Id);
            Invoice = full;
            PaymentMethodSelector.SelectedItem = full?.PaymentMethod;
            SupplierSelector.SelectedItem = full?.Supplier;
            InvoiceItems = new ObservableCollection<InvoiceItemViewModel>(
                full?.Items.Select(i => new InvoiceItemViewModel(i)) ?? new List<InvoiceItemViewModel>());
            foreach (var item in InvoiceItems)
                item.RecalculateAmounts(IsGrossBased);
            OnPropertyChanged(nameof(IsGrossBased));

            if (full != null)
            {
                var totals = await _invoiceService.CalculateTotalsAsync(full);
                InvoiceTotals = new InvoiceTotals
                {
                    TotalNet = Math.Round(totals.TotalNet, 2),
                    TotalVat = Math.Round(totals.TotalVat, 2),
                    TotalGross = Math.Round(totals.TotalGross, 2),
                    ByTaxRate = totals.ByTaxRate
                        .Select(tg => new TaxRateTotal
                        {
                            TaxCode = tg.TaxCode,
                            Net = Math.Round(tg.Net, 2),
                            Vat = Math.Round(tg.Vat, 2),
                            Gross = Math.Round(tg.Gross, 2)
                        })
                        .ToList()
                };
            }
            else
            {
                InvoiceTotals = new InvoiceTotals();
            }
        }

        private async Task RecalculateTotalsAsync()
        {
            if (Invoice == null)
                return;

            var totals = await _invoiceService.CalculateTotalsAsync(Invoice);
            InvoiceTotals = new InvoiceTotals
            {
                TotalNet = Math.Round(totals.TotalNet, 2),
                TotalVat = Math.Round(totals.TotalVat, 2),
                TotalGross = Math.Round(totals.TotalGross, 2),
                ByTaxRate = totals.ByTaxRate
                    .Select(tg => new TaxRateTotal
                    {
                        TaxCode = tg.TaxCode,
                        Net = Math.Round(tg.Net, 2),
                        Vat = Math.Round(tg.Vat, 2),
                        Gross = Math.Round(tg.Gross, 2)
                    })
                    .ToList()
            };
            OnPropertyChanged(nameof(InvoiceItems));
        }

        private void PaymentMethodSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PaymentMethodSelector.SelectedItem) && Invoice != null && PaymentMethodSelector.SelectedItem != null)
            {
                Invoice.PaymentMethod = PaymentMethodSelector.SelectedItem;
                Invoice.PaymentMethodId = PaymentMethodSelector.SelectedItem.Id;
            }
        }

        private void SupplierSelectorOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SupplierSelector.SelectedItem) && Invoice != null && SupplierSelector.SelectedItem != null)
            {
                Invoice.Supplier = SupplierSelector.SelectedItem;
                Invoice.SupplierId = SupplierSelector.SelectedItem.Id;
            }
        }

        private void InputRowOnItemReadyToAdd(InvoiceItem item)
        {
            if (Invoice == null)
                return;

            item.InvoiceId = Invoice.Id;
            item.Invoice = Invoice;
            Invoice.Items.Add(item);
            var vm = new InvoiceItemViewModel(item);
            vm.RecalculateAmounts(IsGrossBased);
            InvoiceItems.Add(vm);
            _ = RecalculateTotalsAsync();
        }

        private bool CanBeginEditItem() => SelectedInvoiceItem != null;

        private void BeginEditItem()
        {
            if (SelectedInvoiceItem != null)
            {
                InputRow.BeginEdit(SelectedInvoiceItem);
            }
        }

        private bool CanDeleteSelectedItem() => SelectedInvoiceItem != null;

        private async void DeleteSelectedItem()
        {
            if (SelectedInvoiceItem == null || Invoice == null)
                return;

            var confirm = await _confirmationService.ConfirmAsync("Delete item", "Are you sure?");
            if (!confirm)
                return;

            if (SelectedInvoiceItem.Item.Id != 0)
                await _invoiceItemService.DeleteAsync(SelectedInvoiceItem.Item.Id);
            Invoice.Items.Remove(SelectedInvoiceItem.Item);
            InvoiceItems.Remove(SelectedInvoiceItem);
            _ = RecalculateTotalsAsync();
        }

        private void UpdateDetailVisibility()
        {
            if (_mainViewModel.SelectedInvoice != null)
            {
                _mainViewModel.ScreenState = InvoiceScreenState.Editing;
            }
            else
            {
                _mainViewModel.ScreenState = InvoiceScreenState.Browsing;
            }
        }
    }
}
