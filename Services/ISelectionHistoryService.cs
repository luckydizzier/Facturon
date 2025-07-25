using System.Collections.Generic;
using System.Threading.Tasks;

namespace Facturon.Services
{
    public interface ISelectionHistoryService
    {
        Task RecordSelectionAsync(string entityName, int id);
        Task<List<int>> GetRecentIdsAsync(string entityName, int count);
    }
}
