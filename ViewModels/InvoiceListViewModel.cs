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

        public InvoiceListViewModel(IInvoiceService invoiceService)
        {
            Debug.WriteLine("InvoiceListViewModel created");
            _invoiceService = invoiceService;
            Invoices = new ObservableCollection<Invoice>();
            Invoices.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasInvoices));
        }

        public async Task InitializeAsync()
        {
            var list = await _invoiceService.GetInvoicesAsync();
            Invoices = new ObservableCollection<Invoice>(list);
            Invoices.CollectionChanged += (_, __) => OnPropertyChanged(nameof(HasInvoices));
            OnPropertyChanged(nameof(Invoices));
            OnPropertyChanged(nameof(HasInvoices));
        }
    }
}
