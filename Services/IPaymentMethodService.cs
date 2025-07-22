using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IPaymentMethodService : IEntityService<PaymentMethod>
    {
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task<Result> UpdateAsync(PaymentMethod method);
        Task<Result> DeleteAsync(int id);
    }
}
