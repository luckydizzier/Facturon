using Facturon.Domain.Entities;
using Facturon.Services;
using Facturon.App.ViewModels.Dialogs;
using Facturon.App.Views.Dialogs;

namespace Facturon.App
{
    public class NewPaymentMethodDialogService : INewEntityDialogService<PaymentMethod>
    {
        public PaymentMethod? ShowDialog()
        {
            var dialog = new NewPaymentMethodDialog();
            var vm = new NewPaymentMethodDialogViewModel();
            dialog.DataContext = vm;
            vm.CloseRequested += m => dialog.DialogResult = m != null;
            var result = dialog.ShowDialog();
            return result == true ? vm.Method : null;
        }
    }
}
