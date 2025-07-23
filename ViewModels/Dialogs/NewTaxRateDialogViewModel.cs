using System;
using System.ComponentModel;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels.Dialogs
{
    public class NewTaxRateDialogViewModel : BaseViewModel, IDataErrorInfo
    {
        public TaxRate Rate { get; } = new()
        {
            Code = string.Empty,
            Value = 0m,
            ValidFrom = DateTime.Today,
            ValidTo = DateTime.Today
        };

        private DateTime? _validTo;
        public DateTime? ValidTo
        {
            get => _validTo;
            set
            {
                if (_validTo != value)
                {
                    _validTo = value;
                    OnPropertyChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

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

        public event Action<TaxRate?>? CloseRequested;

        public NewTaxRateDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private bool CanSave()
        {
            return string.IsNullOrWhiteSpace(Rate.Code) == false
                   && Rate.Value >= 0 && Rate.Value <= 100
                   && (ValidTo == null || Rate.ValidFrom <= ValidTo);
        }

        private void Save()
        {
            Rate.ValidTo = ValidTo ?? DateTime.MaxValue;
            Rate.Active = IsActive;
            CloseRequested?.Invoke(Rate);
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Rate.Code) => string.IsNullOrWhiteSpace(Rate.Code)
                        ? "Required" : string.Empty,
                    nameof(Rate.Value) => Rate.Value < 0 || Rate.Value > 100
                        ? "0-100" : string.Empty,
                    nameof(Rate.ValidFrom) or nameof(ValidTo) =>
                        ValidTo != null && ValidTo < Rate.ValidFrom
                            ? "Invalid range" : string.Empty,
                    _ => string.Empty
                };
            }
        }
    }
}
