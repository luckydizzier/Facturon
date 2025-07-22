using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface ISupplierService : IEntityService<Supplier>
    {
        Task<Supplier?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(Supplier supplier);
        Task<Result> DeleteAsync(int id);
    }
}
