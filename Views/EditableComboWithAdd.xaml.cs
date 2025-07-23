using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Facturon.App.Views
{
    public partial class EditableComboWithAdd : UserControl
    {
        public EditableComboWithAdd()
        {
            InitializeComponent();
            Combo.KeyDown += Combo_KeyDown;
            Combo.LostFocus += Combo_LostFocus;
        }

        private async void Combo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                await ConfirmAsync();
            }
        }

        private async void Combo_LostFocus(object sender, RoutedEventArgs e)
        {
            await ConfirmAsync();
        }

        private async Task ConfirmAsync()
        {
            if (DataContext is null)
                return;

            dynamic vm = DataContext;
            await vm.ConfirmInputAsync();
        }
    }
}
