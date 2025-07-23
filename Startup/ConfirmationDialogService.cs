using System.Threading.Tasks;
using System.Windows;
using Facturon.Services;

namespace Facturon.App
{
    public class ConfirmationDialogService : IConfirmationDialogService
    {
        public Task<bool> ConfirmAsync(string title, string message)
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            return Task.FromResult(result == MessageBoxResult.Yes);
        }
    }
}
