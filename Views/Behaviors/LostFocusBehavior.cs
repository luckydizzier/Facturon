using System.Windows;
using System.Windows.Input;

namespace Facturon.App.Views.Behaviors
{
    public static class LostFocusBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(LostFocusBehavior),
            new PropertyMetadata(null, OnCommandChanged));

        public static ICommand? GetCommand(DependencyObject obj) => (ICommand?)obj.GetValue(CommandProperty);

        public static void SetCommand(DependencyObject obj, ICommand? value) => obj.SetValue(CommandProperty, value);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.LostFocus -= OnLostFocus;
                if (e.NewValue is ICommand)
                    fe.LostFocus += OnLostFocus;
            }
        }

        private static void OnLostFocus(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var command = GetCommand(fe);
            if (command?.CanExecute(null) == true)
                command.Execute(null);
        }
    }
}
