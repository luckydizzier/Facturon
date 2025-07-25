using System.Windows;
using Facturon.Services;

namespace Facturon.App
{
    public class NavigationService : INavigationService
    {
        public void MoveFocus(FocusNavigationDirection direction)
        {
            if (System.Windows.Input.Keyboard.FocusedElement is UIElement element)
            {
                element.MoveFocus(
                    new System.Windows.Input.TraversalRequest(
                        (System.Windows.Input.FocusNavigationDirection)direction));
            }
        }

        public void MoveNext()
        {
            MoveFocus(FocusNavigationDirection.Next);
        }

        public void MovePrevious()
        {
            MoveFocus(FocusNavigationDirection.Previous);
        }
    }
}
