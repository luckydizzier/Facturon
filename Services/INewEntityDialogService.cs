namespace Facturon.Services
{
    public interface INewEntityDialogService<T>
    {
        T? ShowDialog();
    }
}
