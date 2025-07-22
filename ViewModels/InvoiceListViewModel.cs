using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceListViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;

        public ObservableCollection<Invoice> Invoices { get; private set; }

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
            _invoiceService = invoiceService;
            Invoices = new ObservableCollection<Invoice>();
        }

        public async Task InitializeAsync()
        {
            var list = await _invoiceService.GetInvoicesAsync();
            Invoices = new ObservableCollection<Invoice>(list);
            OnPropertyChanged(nameof(Invoices));
        }
    }
}
