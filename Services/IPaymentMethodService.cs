using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IPaymentMethodService
    {
        Task<PaymentMethod?> GetByIdAsync(int id);
        Task<List<PaymentMethod>> GetAllAsync();
        Task<Result> CreateAsync(PaymentMethod method);
        Task<Result> UpdateAsync(PaymentMethod method);
        Task<Result> DeleteAsync(int id);
    }
}
