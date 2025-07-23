using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels.Dialogs;
using Facturon.App.Views.Dialogs;

namespace Facturon.App
{
    public class NewTaxRateDialogService : INewEntityDialogService<TaxRate>
    {
        public TaxRate? ShowDialog()
        {
            var dialog = new NewTaxRateDialog();
            var vm = new NewTaxRateDialogViewModel();
            dialog.DataContext = vm;
            vm.CloseRequested += r => dialog.DialogResult = r != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Rate : null;
        }
    }
}
