using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IEntityService<TEntity> where TEntity : BaseEntity
    {
        Task<List<TEntity>> GetAllAsync();
        Task<List<TEntity>> GetMostRecentAsync(int count);
        Task<Result> CreateAsync(TEntity entity);
    }
}
