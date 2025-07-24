using System.Windows;
using System.Windows.Input;

namespace Facturon.App.Views.Behaviors
{
    public static class LoadedBehavior
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(LoadedBehavior),
            new PropertyMetadata(null, OnCommandChanged));

        public static ICommand? GetCommand(DependencyObject obj) => (ICommand?)obj.GetValue(CommandProperty);

        public static void SetCommand(DependencyObject obj, ICommand? value) => obj.SetValue(CommandProperty, value);

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement fe)
            {
                fe.Loaded -= OnLoaded;
                if (e.NewValue is ICommand)
                    fe.Loaded += OnLoaded;
            }
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            var fe = (FrameworkElement)sender;
            var command = GetCommand(fe);
            if (command?.CanExecute(null) == true)
                command.Execute(null);
        }
    }
}
