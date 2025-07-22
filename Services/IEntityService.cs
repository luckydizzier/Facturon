using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturon.Services
{
    public interface IEntityService<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> CreateAsync(T entity);
    }
}
