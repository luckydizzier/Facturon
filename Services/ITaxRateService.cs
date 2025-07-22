using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface ITaxRateService : IEntityService<TaxRate>
    {
        Task<TaxRate?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(TaxRate rate);
        Task<Result> DeleteAsync(int id);
    }
}
