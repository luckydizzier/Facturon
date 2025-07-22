using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturon.Services
{
    public interface IEntityService<TEntity>
    {
        Task<List<TEntity>> GetAllAsync();
        Task<Result> CreateAsync(TEntity entity);
    }
}
