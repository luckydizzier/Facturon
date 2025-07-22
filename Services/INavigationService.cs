using System.Windows.Input;

namespace Facturon.Services
{
    public interface INavigationService
    {
        void MoveFocus(FocusNavigationDirection direction);
    }
}
