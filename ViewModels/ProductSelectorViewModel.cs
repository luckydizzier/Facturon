using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class ProductSelectorViewModel : EditableComboWithAddViewModel<Product>
    {
        public ProductSelectorViewModel(
            IProductService service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<Product> dialogService)
            : base(service, confirmationService, dialogService)
        {
        }
    }
}
