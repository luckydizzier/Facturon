using System;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels.Dialogs
{
    public class NewPaymentMethodDialogViewModel : BaseViewModel
    {
        public PaymentMethod Method { get; } = new PaymentMethod
        {
            Name = string.Empty
        };

        private string? _description;
        public string? Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event Action<PaymentMethod?>? CloseRequested;

        public NewPaymentMethodDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Method.Name);
        }

        private void Save()
        {
            CloseRequested?.Invoke(Method);
        }
    }
}
