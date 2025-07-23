using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels;
using Facturon.App.Views;

namespace Facturon.App
{
    public class NewSupplierDialogService : INewEntityDialogService<Supplier>
    {
        public Supplier? ShowDialog()
        {
            var dialog = new NewSupplierDialog();
            var vm = new NewSupplierDialogViewModel();
            dialog.DataContext = vm;
            vm.CloseRequested += result => dialog.DialogResult = result;
            var result = dialog.ShowDialog();
            return result == true ? vm.Supplier : null;
        }
    }
}
