using System.Windows.Controls;

namespace Facturon.App.Views
{
    public partial class InvoiceDetailView : UserControl
    {
        public InvoiceDetailView()
        {
            InitializeComponent();
            ItemsGrid.PreviewKeyDown += ItemsGrid_PreviewKeyDown;
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
    }
}
