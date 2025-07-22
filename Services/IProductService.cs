using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IProductService : IEntityService<Product>
    {
        Task<Product?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(Product product);
        Task<Result> DeleteAsync(int id);
    }
}
