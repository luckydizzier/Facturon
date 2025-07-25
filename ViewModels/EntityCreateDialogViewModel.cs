using Facturon.Services;
using Facturon.Domain.Entities;

namespace Facturon.App.ViewModels
{
    public class EntityCreateDialogViewModel<T> : BaseViewModel where T : BaseEntity
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
