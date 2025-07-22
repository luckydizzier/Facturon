using System.Threading.Tasks;
using System.Diagnostics;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;

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

        public MainViewModel(
            IInvoiceService invoiceService,
            IEntityService<Supplier> supplierService,
            IEntityService<Product> productService,
            IEntityService<Unit> unitService,
            IEntityService<TaxRate> taxRateService,
            IEntityService<ProductGroup> productGroupService)
        {
            Debug.WriteLine("MainViewModel created");
            _invoiceService = invoiceService;
            InvoiceList = new InvoiceListViewModel(invoiceService);
            InvoiceList.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(InvoiceListViewModel.SelectedInvoice))
                    OnPropertyChanged(nameof(SelectedInvoice));
            };

            InvoiceDetail = new InvoiceDetailViewModel(
                invoiceService,
                this,
                supplierService,
                productService,
                unitService,
                taxRateService,
                productGroupService);

            OpenInvoiceCommand = new RelayCommand(OpenSelected, CanOpenSelected);
            CloseDetailCommand = new RelayCommand(CloseDetail, () => DetailVisible);
            NewInvoiceCommand = new RelayCommand(NewInvoice);
            DeleteInvoiceCommand = new RelayCommand(DeleteSelected, CanDeleteSelected);
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
    }
}
