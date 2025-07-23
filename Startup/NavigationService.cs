using System.Windows;
using Facturon.Services;

namespace Facturon.App
{
    public class NavigationService : INavigationService
    {
        public void MoveFocus(FocusNavigationDirection direction)
        {
            System.Windows.Input.Keyboard.FocusedElement?.MoveFocus(
                new System.Windows.Input.TraversalRequest(
                    (System.Windows.Input.FocusNavigationDirection)direction));
        }
    }
}
