using System.Windows.Controls;

namespace Facturon.App.Views
{
    public partial class InvoiceDetailView : UserControl
    {
        public InvoiceDetailView()
        {
            InitializeComponent();
            ItemsGrid.PreviewKeyDown += ItemsGrid_PreviewKeyDown;
            ItemsGrid.MouseDoubleClick += ItemsGrid_MouseDoubleClick;
            ItemsGrid.KeyDown += ItemsGrid_KeyDown;
        }

        private void ItemsGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete && DataContext is ViewModels.InvoiceDetailViewModel vm)
            {
                if (vm.DeleteSelectedItemCommand.CanExecute(null))
                {
                    e.Handled = true;
                    vm.DeleteSelectedItemCommand.Execute(null);
                }
            }
        }

        private void ItemsGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (DataContext is ViewModels.InvoiceDetailViewModel vm && vm.SelectedInvoiceItem != null)
            {
                vm.InputRow.BeginEdit(vm.SelectedInvoiceItem);
            }
        }

        private void ItemsGrid_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F2 && DataContext is ViewModels.InvoiceDetailViewModel vm && vm.SelectedInvoiceItem != null)
            {
                e.Handled = true;
                vm.InputRow.BeginEdit(vm.SelectedInvoiceItem);
            }
        }
    }
}
