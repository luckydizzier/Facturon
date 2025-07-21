using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfUnitRepository : BaseRepository<Unit>, IUnitRepository
    {
        public EfUnitRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
