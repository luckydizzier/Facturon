using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class ProductGroupSelectorViewModel : EditableComboWithAddViewModel<ProductGroup>
    {
        public ProductGroupSelectorViewModel(
            IProductGroupService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<ProductGroup> dialogService,
            ISelectionHistoryService historyService)
            : base(service, confirmationService, dialogService, historyService)
        {
        }
    }
}
