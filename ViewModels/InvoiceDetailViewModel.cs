using System.Windows;
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

        // TODO: Toggle DetailVisible when invoice selection changes
    }
}
