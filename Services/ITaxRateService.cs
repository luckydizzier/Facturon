using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface ITaxRateService
    {
        Task<TaxRate?> GetByIdAsync(int id);
        Task<List<TaxRate>> GetAllAsync();
        Task<Result> CreateAsync(TaxRate rate);
        Task<Result> UpdateAsync(TaxRate rate);
        Task<Result> DeleteAsync(int id);
    }
}
