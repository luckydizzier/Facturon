using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<PaymentMethod> _paymentMethodDialogService;
        private readonly IProductService _productService;
        private readonly IUnitService _unitService;
        private readonly ITaxRateService _taxRateService;
        private readonly ISupplierService _supplierService;
        private readonly INewEntityDialogService<Product> _productDialogService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;
        private readonly INewEntityDialogService<TaxRate> _taxDialogService;
        private readonly INewEntityDialogService<Supplier> _supplierDialogService;
        private readonly IInvoiceItemService _invoiceItemService;
        private readonly INavigationService _navigationService;

        public InvoiceListViewModel InvoiceList { get; }
        public InvoiceDetailViewModel InvoiceDetail { get; }

        public Invoice? SelectedInvoice
        {
            get => InvoiceList.SelectedInvoice;
            set
            {
                if (InvoiceList.SelectedInvoice != value)
                {
                    InvoiceList.SelectedInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        private InvoiceScreenState _screenState = InvoiceScreenState.Browsing;
        public InvoiceScreenState ScreenState
        {
            get => _screenState;
            set
            {
                if (_screenState != value)
                {
                    _screenState = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DetailVisible));
                    OpenInvoiceCommand.RaiseCanExecuteChanged();
                    CloseDetailCommand.RaiseCanExecuteChanged();
                    NewInvoiceCommand.RaiseCanExecuteChanged();
                    DeleteInvoiceCommand.RaiseCanExecuteChanged();
                    SaveInvoiceCommand.RaiseCanExecuteChanged();
                    CancelInvoiceCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool DetailVisible => ScreenState != InvoiceScreenState.Browsing;

        public RelayCommand OpenInvoiceCommand { get; }
        public RelayCommand CloseDetailCommand { get; }
        public RelayCommand NewInvoiceCommand { get; }
        public RelayCommand DeleteInvoiceCommand { get; }
        public RelayCommand SaveInvoiceCommand { get; }
        public RelayCommand CancelInvoiceCommand { get; }
        public RelayCommand LoadedCommand { get; }
        public RelayCommand ExitCommand { get; }

        public MainViewModel(
            IInvoiceService invoiceService,
            IPaymentMethodService paymentMethodService,
            IProductService productService,
            IUnitService unitService,
            ITaxRateService taxRateService,
            ISupplierService supplierService,
            IInvoiceItemService invoiceItemService,
            IConfirmationDialogService confirmationService,
            INavigationService navigationService,
            INewEntityDialogService<PaymentMethod> paymentMethodDialogService,
            INewEntityDialogService<Product> productDialogService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxDialogService,
            INewEntityDialogService<Supplier> supplierDialogService)
        {
            Debug.WriteLine("MainViewModel created");
            _invoiceService = invoiceService;
            _paymentMethodService = paymentMethodService;
            _productService = productService;
            _unitService = unitService;
            _taxRateService = taxRateService;
            _supplierService = supplierService;
            _invoiceItemService = invoiceItemService;
            _confirmationService = confirmationService;
            _navigationService = navigationService;
            _paymentMethodDialogService = paymentMethodDialogService;
            _productDialogService = productDialogService;
            _unitDialogService = unitDialogService;
            _taxDialogService = taxDialogService;
            _supplierDialogService = supplierDialogService;
            InvoiceList = new InvoiceListViewModel(invoiceService, navigationService);
            InvoiceList.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(InvoiceListViewModel.SelectedInvoice))
                    OnPropertyChanged(nameof(SelectedInvoice));
            };

            InvoiceDetail = new InvoiceDetailViewModel(
                invoiceService,
                _paymentMethodService,
                _productService,
                _unitService,
                _taxRateService,
                _supplierService,
                _invoiceItemService,
                _confirmationService,
                _paymentMethodDialogService,
                _productDialogService,
                _unitDialogService,
                _taxDialogService,
                _supplierDialogService,
                this);

            OpenInvoiceCommand = new RelayCommand(OpenSelected, () => ScreenState == InvoiceScreenState.Browsing && CanOpenSelected());
            CloseDetailCommand = new RelayCommand(CloseDetail, () => ScreenState != InvoiceScreenState.Browsing);
            NewInvoiceCommand = new RelayCommand(NewInvoice, () => ScreenState == InvoiceScreenState.Browsing);
            DeleteInvoiceCommand = new RelayCommand(DeleteSelected, () => ScreenState == InvoiceScreenState.Browsing && CanDeleteSelected());
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, () => ScreenState == InvoiceScreenState.Editing);
            CancelInvoiceCommand = new RelayCommand(CancelInvoice, () => ScreenState == InvoiceScreenState.Editing);
            LoadedCommand = new RelayCommand(() => _navigationService.MoveFocus(FocusNavigationDirection.First));
            ExitCommand = new RelayCommand(ExecuteExitAsync);
        }

        public async Task InitializeAsync()
        {
            await InvoiceList.InitializeAsync();
        }

        private bool CanOpenSelected() => InvoiceList.SelectedInvoice != null;

        private void OpenSelected()
        {
            if (InvoiceList.SelectedInvoice != null)
            {
                InvoiceDetail.Invoice = InvoiceList.SelectedInvoice;
                ScreenState = InvoiceScreenState.Editing;
            }
        }

        private void CloseDetail()
        {
            InvoiceDetail.Invoice = null;
            ScreenState = InvoiceScreenState.Browsing;
        }

        private void NewInvoice()
        {
            InvoiceDetail.Invoice = new Facturon.Domain.Entities.Invoice
            {
                Date = System.DateTime.Today,
                Issuer = string.Empty,
                Number = string.Empty,
                Supplier = new Facturon.Domain.Entities.Supplier
                {
                    Name = string.Empty,
                    TaxNumber = string.Empty,
                    Address = string.Empty
                },
                PaymentMethod = new Facturon.Domain.Entities.PaymentMethod
                {
                    Name = string.Empty
                }
            };
            InvoiceDetail.SupplierSelector.SelectedItem = InvoiceDetail.SupplierSelector.Items.FirstOrDefault();
            InvoiceDetail.PaymentMethodSelector.SelectedItem = InvoiceDetail.PaymentMethodSelector.Items.FirstOrDefault();
            ScreenState = InvoiceScreenState.Editing;
        }

        private bool CanDeleteSelected() => InvoiceList.SelectedInvoice != null;

        private async void DeleteSelected()
        {
            if (InvoiceList.SelectedInvoice != null)
            {
                await _invoiceService.DeleteAsync(InvoiceList.SelectedInvoice.Id);
                InvoiceList.Invoices.Remove(InvoiceList.SelectedInvoice);
                CloseDetail();
            }
        }

        private async void SaveInvoice()
        {
            if (InvoiceDetail.Invoice == null)
                return;

            Result result;

            if (InvoiceDetail.Invoice.Id == 0)
            {
                result = await _invoiceService.CreateAsync(InvoiceDetail.Invoice);
                if (result.Success)
                {
                    InvoiceList.Invoices.Add(InvoiceDetail.Invoice);
                    SelectedInvoice = InvoiceDetail.Invoice;
                }
            }
            else
            {
                result = await _invoiceService.UpdateAsync(InvoiceDetail.Invoice);
            }

            if (result.Success)
            {
                CloseDetail();
            }
            else
            {
                await _confirmationService.ConfirmAsync(
                    "Save failed",
                    string.IsNullOrEmpty(result.Message) ? "Validation errors occurred." : result.Message);
                ScreenState = InvoiceScreenState.Editing;
            }
        }

        private void CancelInvoice()
        {
            CloseDetail();
        }

        private async void ExecuteExitAsync()
        {
            var confirm = await _confirmationService.ConfirmAsync("Exit", "Do you really want to exit?");
            if (confirm)
                Application.Current?.MainWindow?.Close();
        }
    }
}
