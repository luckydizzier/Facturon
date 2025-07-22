using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Facturon.Services;
using Facturon.App.Views.Controls;
using System;

namespace Facturon.App.ViewModels
{
    public class EditableComboWithAddViewModel<T> : BaseViewModel, INewItemCommandSource
        where T : class
    {
        private readonly IEntityService<T> _service;

        public ObservableCollection<T> AllItems { get; }

        private T? _selectedItem;
        public T? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Equals(_selectedItem, value))
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _typedText = string.Empty;
        public string TypedText
        {
            get => _typedText;
            set
            {
                if (_typedText != value)
                {
                    _typedText = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isCreatingNew;
        public bool IsCreatingNew
        {
            get => _isCreatingNew;
            set
            {
                if (_isCreatingNew != value)
                {
                    _isCreatingNew = value;
                    OnPropertyChanged();
                }
            }
        }

        private T? _newItem;
        public T? NewItem
        {
            get => _newItem;
            set
            {
                if (!Equals(_newItem, value))
                {
                    _newItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CheckIfNewItemCommand { get; }
        public ICommand CreateNewItemCommand { get; }
        public ICommand SaveNewItemCommand { get; }
        public ICommand CancelNewItemCommand { get; }

        public EditableComboWithAddViewModel(IEntityService<T> service)
        {
            _service = service;
            AllItems = new ObservableCollection<T>(service.GetAllAsync().Result);

            CheckIfNewItemCommand = new RelayCommand(CheckIfNewItem);
            CreateNewItemCommand = new RelayCommand(StartCreate);
            SaveNewItemCommand = new RelayCommand(SaveNew);
            CancelNewItemCommand = new RelayCommand(CancelCreate);
        }

        private void CheckIfNewItem()
        {
            if (string.IsNullOrWhiteSpace(TypedText))
                return;

            if (SelectedItem != null && AllItems.Contains(SelectedItem))
                return;

            var result = MessageBox.Show($"\"{TypedText}\" nem létezik. Új elem?",
                "Új", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
                StartCreate();
        }

        private void StartCreate()
        {
            NewItem = Activator.CreateInstance<T>();
            IsCreatingNew = true;
        }

        private async void SaveNew()
        {
            if (NewItem == null)
                return;

            var created = await _service.CreateAsync(NewItem);
            AllItems.Add(created);
            SelectedItem = created;
            IsCreatingNew = false;
            TypedText = string.Empty;
        }

        private void CancelCreate()
        {
            IsCreatingNew = false;
            NewItem = null;
            TypedText = string.Empty;
        }
    }
}
