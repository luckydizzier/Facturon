using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class UnitSelectorViewModel : EditableComboWithAddViewModel<Unit>
    {
        public UnitSelectorViewModel(
            IUnitService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Unit> dialogService)
            : base(service, confirmationService, dialogService)
        {
        }
    }
}
