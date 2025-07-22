using System.Windows;

namespace Facturon.App.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;
            PreviewKeyDown += Window_PreviewKeyDown;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InvoiceListHost.InvoiceList.Focus();
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                var result = MessageBox.Show(
                    "Are you sure you want to exit?",
                    "Confirm Exit",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question,
                    MessageBoxResult.No);
                if (result == MessageBoxResult.Yes)
                    Close();
            }
        }
    }
}
