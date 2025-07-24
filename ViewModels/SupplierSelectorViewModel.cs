using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class SupplierSelectorViewModel : EditableComboWithAddViewModel<Supplier>
    {
        public SupplierSelectorViewModel(
            ISupplierService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Supplier> dialogService)
            : base(service, confirmationService, dialogService)
        {
        }
    }
}
