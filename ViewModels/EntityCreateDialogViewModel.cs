using Facturon.Services;
namespace Facturon.App.ViewModels
{
    public class EntityCreateDialogViewModel<T> : BaseViewModel
    {
        private readonly IEntityService<T> _service;

        public string Name { get; set; } = string.Empty;

        public EntityCreateDialogViewModel(IEntityService<T> service)
        {
            _service = service;
        }

        // Additional validation and save logic can be implemented here
    }
}
