using System;
using System.Windows.Input;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels
{
    public class NewSupplierDialogViewModel : BaseViewModel
    {
        public Supplier Supplier { get; set; } = new()
        {
            Name = string.Empty,
            Address = string.Empty,
            TaxNumber = string.Empty
        };

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action<bool>? CloseRequested;

        public NewSupplierDialogViewModel()
        {
            SaveCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }
    }
}
