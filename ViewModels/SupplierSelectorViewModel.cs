using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class SupplierSelectorViewModel : EditableComboWithAddViewModel<Supplier>
    {
        public SupplierSelectorViewModel(
            ISupplierService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Supplier> dialogService,
            ISelectionHistoryService historyService)
            : base(service, confirmationService, dialogService, historyService)
        {
        }
    }
}
