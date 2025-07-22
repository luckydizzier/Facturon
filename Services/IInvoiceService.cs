using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IInvoiceService
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<List<Invoice>> GetAllAsync();
        Task<List<Invoice>> GetInvoicesAsync();
        Task<Result> CreateAsync(Invoice invoice);
        Task<Result> UpdateAsync(Invoice invoice);
        Task<Result> DeleteAsync(int id);
        Task<InvoiceTotals> CalculateTotalsAsync(Invoice invoice);
    }
}
