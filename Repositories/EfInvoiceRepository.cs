using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfInvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public EfInvoiceRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
