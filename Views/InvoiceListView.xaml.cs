using System.Windows.Controls;
using System.Windows;

namespace Facturon.App.Views
{
    public partial class InvoiceListView : UserControl
    {
        public InvoiceListView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (InvoiceList.Items.Count > 0 && InvoiceList.SelectedIndex == -1)
                InvoiceList.SelectedIndex = 0;
            InvoiceList.Focus();
        }
    }
}
