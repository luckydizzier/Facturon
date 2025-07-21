using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class InvoiceListViewModel : BaseViewModel
    {
        private readonly IInvoiceService _invoiceService;

        public ObservableCollection<Invoice> Invoices { get; } = new();

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
        }

        public async Task LoadAsync()
        {
            Invoices.Clear();
            var list = await _invoiceService.GetAllAsync();
            foreach (var invoice in list)
            {
                Invoices.Add(invoice);
            }
        }
    }
}
