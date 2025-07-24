using System.Windows;
using Facturon.App.ViewModels;

namespace Facturon.App.Views
{
    public partial class ConfirmExitWindow : Window
    {
        public ConfirmExitWindow()
        {
            InitializeComponent();
            var vm = new ConfirmExitViewModel();
            DataContext = vm;
            vm.CloseRequested += result => DialogResult = result;
        }
    }
}
