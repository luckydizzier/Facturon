using System;

namespace Facturon.App.ViewModels
{
    public class ConfirmExitViewModel : BaseViewModel
    {
        public RelayCommand<bool> CloseCommand { get; }

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set
            {
                if (_dialogResult != value)
                {
                    _dialogResult = value;
                    OnPropertyChanged();
                }
            }
        }

        public ConfirmExitViewModel()
        {
            CloseCommand = new RelayCommand<bool>(result => DialogResult = result);
        }
    }
}
