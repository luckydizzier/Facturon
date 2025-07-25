using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels;
using Facturon.App.Views;

namespace Facturon.App
{
    public class NewSupplierDialogService : INewEntityDialogService<Supplier>
    {
        private readonly ISupplierService _service;

        public NewSupplierDialogService(ISupplierService service)
        {
            _service = service;
        }

        public Supplier? ShowDialog()
        {
            var dialog = new NewSupplierDialog();
            var vm = new NewSupplierDialogViewModel(_service);
            dialog.DataContext = vm;
            vm.CloseRequested += result => dialog.DialogResult = result;
            var result = dialog.ShowDialog();
            return result == true ? vm.Supplier : null;
        }
    }
}
