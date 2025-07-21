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
    }
}
