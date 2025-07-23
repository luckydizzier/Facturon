using System.Windows.Controls;
using System.Windows.Input;

namespace Facturon.App.Views
{
    public partial class InvoiceItemInputView : UserControl
    {
        public InvoiceItemInputView()
        {
            InitializeComponent();
            PreviewKeyDown += InvoiceItemInputView_PreviewKeyDown;
        }

        private void InvoiceItemInputView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is not ViewModels.InvoiceItemInputViewModel vm)
                return;

            if (e.Key == Key.Enter)
            {
                if (vm.AddCommand.CanExecute(null))
                {
                    e.Handled = true;
                    vm.AddCommand.Execute(null);
                }
            }
            else if (e.Key == Key.Escape)
            {
                e.Handled = true;
                vm.ClearCommand.Execute(null);
            }
        }
    }
}
