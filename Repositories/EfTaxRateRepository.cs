using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfTaxRateRepository : BaseRepository<TaxRate>, ITaxRateRepository
    {
        public EfTaxRateRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
