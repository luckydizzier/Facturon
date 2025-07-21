using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IUnitService
    {
        Task<Unit?> GetByIdAsync(int id);
        Task<List<Unit>> GetAllAsync();
        Task<Result> CreateAsync(Unit unit);
        Task<Result> UpdateAsync(Unit unit);
        Task<Result> DeleteAsync(int id);
    }
}
