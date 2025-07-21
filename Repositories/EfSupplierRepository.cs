using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfSupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public EfSupplierRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
