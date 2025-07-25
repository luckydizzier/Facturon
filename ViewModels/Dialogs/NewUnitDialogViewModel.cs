using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels.Dialogs
{
    public class NewUnitDialogViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly HashSet<string> _existingNames;
        private readonly HashSet<string> _existingShortNames;

        public Unit Unit { get; } = new Unit
        {
            Name = string.Empty,
            ShortName = string.Empty
        };

        public string Name
        {
            get => Unit.Name;
            set
            {
                if (Unit.Name != value)
                {
                    Unit.Name = value;
                    OnPropertyChanged();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string ShortName
        {
            get => Unit.ShortName;
            set
            {
                if (Unit.ShortName != value)
                {
                    Unit.ShortName = value;
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

        public event Action<Unit?>? CloseRequested;

        public NewUnitDialogViewModel(IUnitService unitService)
        {
            var all = unitService.GetAllAsync().Result;
            _existingNames = all.Select(u => u.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            _existingShortNames = all.Select(u => u.ShortName).ToHashSet(StringComparer.OrdinalIgnoreCase);

            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private bool CanSave()
        {
            return string.IsNullOrEmpty(this[nameof(Name)])
                   && string.IsNullOrEmpty(this[nameof(ShortName)]);
        }

        private void Save()
        {
            Unit.Active = IsActive;
            CloseRequested?.Invoke(Unit);
        }

        public string Error => string.Empty;

        public string this[string columnName]
        {
            get
            {
                return columnName switch
                {
                    nameof(Name) => string.IsNullOrWhiteSpace(Name)
                        ? "Required"
                        : _existingNames.Contains(Name) ? "Name exists" : string.Empty,
                    nameof(ShortName) => string.IsNullOrWhiteSpace(ShortName)
                        ? "Required"
                        : _existingShortNames.Contains(ShortName) ? "Code exists" : string.Empty,
                    _ => string.Empty
                };
            }
        }
    }
}
