using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IProductGroupService
    {
        Task<ProductGroup?> GetByIdAsync(int id);
        Task<List<ProductGroup>> GetAllAsync();
        Task<Result> CreateAsync(ProductGroup group);
        Task<Result> UpdateAsync(ProductGroup group);
        Task<Result> DeleteAsync(int id);
    }
}
