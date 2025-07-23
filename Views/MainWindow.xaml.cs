using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Facturon.Services;
using Facturon.App;

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
            var nav = ((App)Application.Current).Host?.Services.GetRequiredService<INavigationService>();
            nav?.MoveFocus(FocusNavigationDirection.First);
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                e.Handled = true;
                var dlg = new ConfirmExitWindow { Owner = this };
                var result = dlg.ShowDialog();
                if (result == true)
                    Close();
            }
        }
    }
}
