using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Services;

namespace Facturon.App.ViewModels
{
    public class EditableComboWithAddViewModel<T> : BaseViewModel
    {
        private readonly IEntityService<T> _service;

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

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
                }
            }
        }

        public EditableComboWithAddViewModel(IEntityService<T> service)
        {
            _service = service;
        }

        public async Task InitializeAsync()
        {
            var items = await _service.GetAllAsync();
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
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

            // Confirmation and creation dialog logic to be implemented
        }
    }
}
