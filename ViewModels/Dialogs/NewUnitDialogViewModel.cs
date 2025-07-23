using System;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels.Dialogs
{
    public class NewUnitDialogViewModel : BaseViewModel
    {
        public Unit Unit { get; } = new Unit
        {
            Name = string.Empty,
            ShortName = string.Empty
        };

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event Action<Unit?>? CloseRequested;

        public NewUnitDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Unit.Name)
                && !string.IsNullOrWhiteSpace(Unit.ShortName);
        }

        private void Save()
        {
            Unit.Active = IsActive;
            CloseRequested?.Invoke(Unit);
        }
    }
}
