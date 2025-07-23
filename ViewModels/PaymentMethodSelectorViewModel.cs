using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class PaymentMethodSelectorViewModel : EditableComboWithAddViewModel<PaymentMethod>
    {
        public PaymentMethodSelectorViewModel(
            IPaymentMethodService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<PaymentMethod> dialogService)
            : base(service, confirmationService, dialogService)
        {
        }
    }
}
