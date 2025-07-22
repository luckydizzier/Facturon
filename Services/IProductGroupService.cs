using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IProductGroupService : IEntityService<ProductGroup>
    {
        Task<ProductGroup?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(ProductGroup group);
        Task<Result> DeleteAsync(int id);
    }
}
