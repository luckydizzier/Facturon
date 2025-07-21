using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfProductGroupRepository : BaseRepository<ProductGroup>, IProductGroupRepository
    {
        public EfProductGroupRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
