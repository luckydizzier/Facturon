using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels.Dialogs;
using Facturon.App.Views.Dialogs;

namespace Facturon.App
{
    public class NewUnitDialogService : INewEntityDialogService<Unit>
    {
        public Unit? ShowDialog()
        {
            var dialog = new NewUnitDialog();
            var vm = new NewUnitDialogViewModel();
            dialog.DataContext = vm;
            vm.CloseRequested += u => dialog.DialogResult = u != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Unit : null;
        }
    }
}
