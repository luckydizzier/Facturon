using Facturon.Data;
using Facturon.Domain.Entities;

namespace Facturon.Repositories
{
    public class EfPaymentMethodRepository : BaseRepository<PaymentMethod>, IPaymentMethodRepository
    {
        public EfPaymentMethodRepository(FacturonDbContext context) : base(context)
        {
        }
    }
}
