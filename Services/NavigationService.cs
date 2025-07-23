using System.Windows;
using System.Windows.Input;

namespace Facturon.Services
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
