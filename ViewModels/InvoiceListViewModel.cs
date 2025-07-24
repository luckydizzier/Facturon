using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Diagnostics;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceListViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;
        private readonly INavigationService _navigationService;

        public ObservableCollection<Invoice> Invoices { get; private set; }

        public bool HasInvoices => Invoices.Count > 0;

        private Invoice? _selectedInvoice;
        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                if (_selectedInvoice != value)
                {
                    _selectedInvoice = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand LoadedCommand { get; }

        public InvoiceListViewModel(IInvoiceService invoiceService, INavigationService navigationService)
        {
            Debug.WriteLine("InvoiceListViewModel created");
            _invoiceService = invoiceService;
            _navigationService = navigationService;
            Invoices = new ObservableCollection<Invoice>();
            Invoices.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasInvoices));
            LoadedCommand = new RelayCommand(OnLoaded);
        }

        public async Task InitializeAsync()
        {
            var list = await _invoiceService.GetInvoicesAsync();
            Invoices = new ObservableCollection<Invoice>(list);
            Invoices.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasInvoices));
            OnPropertyChanged(nameof(Invoices));
            OnPropertyChanged(nameof(HasInvoices));
        }

        private void OnLoaded()
        {
            if (Invoices.Count > 0 && SelectedInvoice == null)
                SelectedInvoice = Invoices[0];
            _navigationService.MoveFocus(FocusNavigationDirection.First);
        }
    }
}
