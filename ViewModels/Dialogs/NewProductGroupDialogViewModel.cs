using System;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels.Dialogs
{
    public class NewProductGroupDialogViewModel : BaseViewModel
    {
        public ProductGroup Group { get; } = new ProductGroup
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

        public event Action<ProductGroup?>? CloseRequested;

        public NewProductGroupDialogViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(null));
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Group.Name);
        }

        private void Save()
        {
            CloseRequested?.Invoke(Group);
        }
    }
}
