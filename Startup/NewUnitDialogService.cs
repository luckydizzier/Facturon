using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels.Dialogs;
using Facturon.App.Views.Dialogs;

namespace Facturon.App
{
    public class NewUnitDialogService : INewEntityDialogService<Unit>
    {
        private readonly IUnitService _service;

        public NewUnitDialogService(IUnitService service)
        {
            _service = service;
        }

        public Unit? ShowDialog()
        {
            var dialog = new NewUnitDialog();
            var vm = new NewUnitDialogViewModel(_service);
            dialog.DataContext = vm;
            vm.CloseRequested += u => dialog.DialogResult = u != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Unit : null;
        }
    }
}
