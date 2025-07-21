using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface ISupplierService
    {
        Task<Supplier?> GetByIdAsync(int id);
        Task<List<Supplier>> GetAllAsync();
        Task<Result> CreateAsync(Supplier supplier);
        Task<Result> UpdateAsync(Supplier supplier);
        Task<Result> DeleteAsync(int id);
    }
}
