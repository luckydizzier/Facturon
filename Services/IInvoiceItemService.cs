using System.Collections.Generic;
using System.Threading.Tasks;
using Facturon.Domain.Entities;

namespace Facturon.Services
{
    public interface IInvoiceItemService
    {
        Task<InvoiceItem?> GetByIdAsync(int id);
        Task<List<InvoiceItem>> GetByInvoiceAsync(int invoiceId);
        Task<Result> CreateAsync(InvoiceItem item);
        Task<Result> UpdateAsync(InvoiceItem item);
        Task<Result> DeleteAsync(int id);
    }
}
