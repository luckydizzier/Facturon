using System.Windows.Controls;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Facturon.Services;
using Facturon.App;

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
            var nav = ((App)Application.Current).Host?.Services.GetRequiredService<INavigationService>();
            nav?.MoveFocus(FocusNavigationDirection.First);
        }
    }
}
