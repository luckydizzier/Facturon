using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Services;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels
{
    public class EditableComboWithAddViewModel<T> : BaseViewModel where T : BaseEntity
    {
        private readonly IEntityService<T> _service;
        private readonly IConfirmationDialogService _confirmationService;
        private readonly INewEntityDialogService<T> _dialogService;
        private readonly ISelectionHistoryService _historyService;

        public event EventHandler? FocusRequested;

        public RelayCommand ConfirmInputCommand { get; }
        public RelayCommand AddNewCommand { get; }

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();
        public ICollectionView FilteredItems { get; }

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
                    if (value != null)
                        _ = _historyService.RecordSelectionAsync(typeof(T).Name, value.Id);
                }
            }
        }

        private string _input = string.Empty;
        public string Input
        {
            get => _input;
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged();
                    FilteredItems.Refresh();
                }
            }
        }

        public EditableComboWithAddViewModel(
            IEntityService<T> service,
            IConfirmationDialogService confirmationService,
            INewEntityDialogService<T> dialogService,
            ISelectionHistoryService historyService)
        {
            _service = service;
            _confirmationService = confirmationService;
            _dialogService = dialogService;
            _historyService = historyService;
            ConfirmInputCommand = new RelayCommand(ExecuteConfirmInputAsync);
            AddNewCommand = new RelayCommand(ExecuteAddNewAsync);
            FilteredItems = CollectionViewSource.GetDefaultView(Items);
            FilteredItems.Filter = FilterItem;
        }

        private bool FilterItem(object obj)
        {
            if (string.IsNullOrEmpty(Input))
                return true;

            if (obj is T item)
                return item?.ToString()?.IndexOf(Input, StringComparison.OrdinalIgnoreCase) >= 0;

            return false;
        }

        private async void ExecuteConfirmInputAsync()
        {
            await ConfirmInputAsync();
        }

        public async Task InitializeAsync()
        {
            Items.Clear();
            var recent = await _service.GetMostRecentAsync(5);
            foreach (var item in recent)
                Items.Add(item);
            var all = await _service.GetAllAsync();
            foreach (var item in all.Where(a => Items.All(i => i.Id != a.Id)))
                Items.Add(item);
            FilteredItems.Refresh();
        }

        public async Task ConfirmInputAsync()
        {
            if (string.IsNullOrWhiteSpace(Input))
                return;

            var existing = Items.FirstOrDefault(i => i?.ToString() == Input);
            if (existing != null)
            {
                SelectedItem = existing;
                return;
            }

            var confirm = await _confirmationService.ConfirmAsync("Új elem?", $"A(z) '{Input}' nem található. Létrehozza?");
            if (!confirm)
            {
                FocusRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            var newEntity = _dialogService.ShowDialog();
            if (newEntity == null)
            {
                FocusRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            var result = await _service.CreateAsync(newEntity);
            if (result.Success)
            {
                Items.Add(newEntity);
                FilteredItems.Refresh();
                SelectedItem = newEntity;
            }
            else
            {
                FocusRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        private async void ExecuteAddNewAsync()
        {
            var newEntity = _dialogService.ShowDialog();
            if (newEntity == null)
            {
                FocusRequested?.Invoke(this, EventArgs.Empty);
                return;
            }

            var result = await _service.CreateAsync(newEntity);
            if (result.Success)
            {
                Items.Add(newEntity);
                FilteredItems.Refresh();
                SelectedItem = newEntity;
            }
            else
            {
                FocusRequested?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
