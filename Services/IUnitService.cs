using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IUnitService : IEntityService<Unit>
    {
        Task<Unit?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(Unit unit);
        Task<Result> DeleteAsync(int id);
    }
}
