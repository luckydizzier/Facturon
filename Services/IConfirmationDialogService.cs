namespace Facturon.Services
{
    public interface IConfirmationDialogService
    {
        Task<bool> ConfirmAsync(string title, string message);
    }
}
