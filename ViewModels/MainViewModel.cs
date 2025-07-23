using System.Threading.Tasks;
using System.Diagnostics;
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
        private readonly INewEntityDialogService<Product> _productDialogService;
        private readonly INewEntityDialogService<Unit> _unitDialogService;
        private readonly INewEntityDialogService<TaxRate> _taxDialogService;
        private readonly IInvoiceItemService _invoiceItemService;

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

        private bool _detailVisible;
        public bool DetailVisible
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

        public RelayCommand OpenInvoiceCommand { get; }
        public RelayCommand CloseDetailCommand { get; }
        public RelayCommand NewInvoiceCommand { get; }
        public RelayCommand DeleteInvoiceCommand { get; }
        public RelayCommand SaveInvoiceCommand { get; }
        public RelayCommand CancelInvoiceCommand { get; }

        public MainViewModel(
            IInvoiceService invoiceService,
            IPaymentMethodService paymentMethodService,
            IProductService productService,
            IUnitService unitService,
            ITaxRateService taxRateService,
            IInvoiceItemService invoiceItemService,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<PaymentMethod> paymentMethodDialogService,
            INewEntityDialogService<Product> productDialogService,
            INewEntityDialogService<Unit> unitDialogService,
            INewEntityDialogService<TaxRate> taxDialogService)
        {
            Debug.WriteLine("MainViewModel created");
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
            InvoiceList = new InvoiceListViewModel(invoiceService);
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
                _invoiceItemService,
                _confirmationService,
                _paymentMethodDialogService,
                _productDialogService,
                _unitDialogService,
                _taxDialogService,
                this);

            OpenInvoiceCommand = new RelayCommand(OpenSelected, CanOpenSelected);
            CloseDetailCommand = new RelayCommand(CloseDetail, () => DetailVisible);
            NewInvoiceCommand = new RelayCommand(NewInvoice);
            DeleteInvoiceCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
            SaveInvoiceCommand = new RelayCommand(SaveInvoice, () => DetailVisible);
            CancelInvoiceCommand = new RelayCommand(CancelInvoice, () => DetailVisible);
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
                DetailVisible = true;
                DeleteInvoiceCommand.RaiseCanExecuteChanged();
                CloseDetailCommand.RaiseCanExecuteChanged();
            }
        }

        private void CloseDetail()
        {
            InvoiceDetail.Invoice = null;
            DetailVisible = false;
            CloseDetailCommand.RaiseCanExecuteChanged();
            DeleteInvoiceCommand.RaiseCanExecuteChanged();
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
            DetailVisible = true;
            CloseDetailCommand.RaiseCanExecuteChanged();
            DeleteInvoiceCommand.RaiseCanExecuteChanged();
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

            if (InvoiceDetail.Invoice.Id == 0)
            {
                var result = await _invoiceService.CreateAsync(InvoiceDetail.Invoice);
                if (result.Success)
                {
                    InvoiceList.Invoices.Add(InvoiceDetail.Invoice);
                    SelectedInvoice = InvoiceDetail.Invoice;
                    CloseDetail();
                }
            }
            else
            {
                var result = await _invoiceService.UpdateAsync(InvoiceDetail.Invoice);
                if (result.Success)
                {
                    CloseDetail();
                }
            }
        }

        private void CancelInvoice()
        {
            CloseDetail();
        }
    }
}
