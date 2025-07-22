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
        }

        public static readonly DependencyProperty InlineCreateTemplateProperty = DependencyProperty.Register(
            nameof(InlineCreateTemplate), typeof(DataTemplate), typeof(EditableComboWithAdd), new PropertyMetadata(null));

        public DataTemplate? InlineCreateTemplate
        {
            get => (DataTemplate?)GetValue(InlineCreateTemplateProperty);
            set => SetValue(InlineCreateTemplateProperty, value);
        }

        private void ComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (DataContext is ICommandSource cmdSrc && cmdSrc.CheckIfNewItemCommand?.CanExecute(null) == true)
                {
                    cmdSrc.CheckIfNewItemCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (DataContext is ICommandSource cmdSrc && cmdSrc.CheckIfNewItemCommand?.CanExecute(null) == true)
            {
                cmdSrc.CheckIfNewItemCommand.Execute(null);
            }
        }
    }

    public interface ICommandSource
    {
        ICommand CheckIfNewItemCommand { get; }
    }
}
