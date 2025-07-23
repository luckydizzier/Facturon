using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels.Dialogs;
using Facturon.App.Views.Dialogs;

namespace Facturon.App
{
    public class NewProductGroupDialogService : INewEntityDialogService<ProductGroup>
    {
        public ProductGroup? ShowDialog()
        {
            var dialog = new NewProductGroupDialog();
            var vm = new NewProductGroupDialogViewModel();
            dialog.DataContext = vm;
            vm.CloseRequested += g => dialog.DialogResult = g != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Group : null;
        }
    }
}
