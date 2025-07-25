using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Facturon.Domain.Entities;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class NewSupplierDialogViewModel : BaseViewModel, IDataErrorInfo
    {
        private readonly HashSet<string> _existingNames;
        private readonly HashSet<string> _existingTaxNumbers;

        public Supplier Supplier { get; } = new()
        {
            Name = string.Empty,
            Address = string.Empty,
            TaxNumber = string.Empty
        };

        public string Name
        {
            get => Supplier.Name;
            set
            {
                if (Supplier.Name != value)
                {
                    Supplier.Name = value;
                    OnPropertyChanged();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string Address
        {
            get => Supplier.Address;
            set
            {
                if (Supplier.Address != value)
                {
                    Supplier.Address = value;
                    OnPropertyChanged();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public string TaxNumber
        {
            get => Supplier.TaxNumber;
            set
            {
                if (Supplier.TaxNumber != value)
                {
                    Supplier.TaxNumber = value;
                    OnPropertyChanged();
                    ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public event Action<bool>? CloseRequested;

        public NewSupplierDialogViewModel(ISupplierService supplierService)
        {
            var all = supplierService.GetAllAsync().Result;
            _existingNames = all.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            _existingTaxNumbers = all.Select(s => s.TaxNumber).ToHashSet(StringComparer.OrdinalIgnoreCase);

            SaveCommand = new RelayCommand(() => CloseRequested?.Invoke(true), CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(false));
        }

        private bool CanSave()
        {
            return string.IsNullOrEmpty(this[nameof(Name)])
                   && string.IsNullOrEmpty(this[nameof(Address)])
                   && string.IsNullOrEmpty(this[nameof(TaxNumber)]);
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
                    nameof(Address) => string.IsNullOrWhiteSpace(Address)
                        ? "Required" : string.Empty,
                    nameof(TaxNumber) => string.IsNullOrWhiteSpace(TaxNumber)
                        ? "Required"
                        : _existingTaxNumbers.Contains(TaxNumber) ? "Tax number exists" : string.Empty,
                    _ => string.Empty
                };
            }
        }
    }
}
