namespace Facturon.Services
{
    public interface INavigationService
    {
        void MoveFocus(FocusNavigationDirection direction);
        void MoveNext();
        void MovePrevious();
    }
}
