using System.Windows;
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
        private readonly MainViewModel _mainViewModel;

        public InvoiceDetailViewModel(IInvoiceService invoiceService, MainViewModel mainViewModel)
        {
            _invoiceService = invoiceService;
            _mainViewModel = mainViewModel;
            InvoiceItems = new ObservableCollection<InvoiceItem>();
            _mainViewModel.PropertyChanged += MainViewModel_PropertyChanged;
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

        private ObservableCollection<InvoiceItem> _invoiceItems;
        public ObservableCollection<InvoiceItem> InvoiceItems
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

        private async void MainViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedInvoice))
                await LoadInvoiceAsync();
        }

        private async Task LoadInvoiceAsync()
        {
            if (_mainViewModel.SelectedInvoice == null)
            {
                Invoice = null;
                InvoiceItems.Clear();
                return;
            }

            var full = await _invoiceService.GetByIdAsync(_mainViewModel.SelectedInvoice.Id);
            Invoice = full;
            InvoiceItems = new ObservableCollection<InvoiceItem>(full?.Items ?? new List<InvoiceItem>());
        }

        // TODO: Toggle DetailVisible when invoice selection changes
    }
}
