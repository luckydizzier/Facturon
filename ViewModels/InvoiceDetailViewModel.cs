using System.Windows;
using System.Collections.Generic;
using System.Linq;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels
{
    public class InvoiceDetailViewModel : BaseViewModel
    {
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

        public IEnumerable<InvoiceItem> InvoiceItems => Invoice?.Items ?? Enumerable.Empty<InvoiceItem>();

        // TODO: Toggle DetailVisible when invoice selection changes
    }
}
