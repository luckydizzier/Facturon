using System;

namespace Facturon.App.ViewModels
{
    public class ConfirmExitViewModel : BaseViewModel
    {
        public RelayCommand YesCommand { get; }
        public RelayCommand NoCommand { get; }

        public event Action<bool>? CloseRequested;

        public ConfirmExitViewModel()
        {
            YesCommand = new RelayCommand(() => CloseRequested?.Invoke(true));
            NoCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }
    }
}
