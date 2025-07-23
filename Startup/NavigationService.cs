using System.Windows;
using System.Windows.Input;
using Facturon.Services;

namespace Facturon.App
{
    public class NavigationService : INavigationService
    {
        public void MoveFocus(FocusNavigationDirection direction)
        {
            Keyboard.FocusedElement?.MoveFocus(
                new TraversalRequest((System.Windows.Input.FocusNavigationDirection)direction));
        }
    }
}
