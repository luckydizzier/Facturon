using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfProductRepository : BaseRepository<Product>, IProductRepository
    {
        public EfProductRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
