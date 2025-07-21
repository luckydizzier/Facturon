using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IProductService
    {
        Task<Product?> GetByIdAsync(int id);
        Task<List<Product>> GetAllAsync();
        Task<Result> CreateAsync(Product product);
        Task<Result> UpdateAsync(Product product);
        Task<Result> DeleteAsync(int id);
    }
}
