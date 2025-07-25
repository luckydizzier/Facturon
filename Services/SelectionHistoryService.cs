using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Facturon.Services
{
    public class SelectionHistoryService : ISelectionHistoryService
    {
        private readonly string _filePath;
        private readonly SemaphoreSlim _lock = new(1, 1);

        private record Entry(int Id, DateTime Date);

        public SelectionHistoryService()
        {
            var baseDir = AppContext.BaseDirectory;
            _filePath = Path.Combine(baseDir, "selectionhistory.json");
        }

        private async Task<Dictionary<string, List<Entry>>> LoadAsync()
        {
            if (!File.Exists(_filePath))
                return new();
            var json = await File.ReadAllTextAsync(_filePath);
            return JsonSerializer.Deserialize<Dictionary<string, List<Entry>>>(json) ?? new();
        }

        private async Task SaveAsync(Dictionary<string, List<Entry>> data)
        {
            var json = JsonSerializer.Serialize(data);
            await File.WriteAllTextAsync(_filePath, json);
        }

        public async Task RecordSelectionAsync(string entityName, int id)
        {
            await _lock.WaitAsync();
            try
            {
                var data = await LoadAsync();
                if (!data.TryGetValue(entityName, out var list))
                {
                    list = new List<Entry>();
                    data[entityName] = list;
                }

                var existing = list.FirstOrDefault(e => e.Id == id);
                if (existing != null)
                {
                    list.Remove(existing);
                }
                list.Insert(0, new Entry(id, DateTime.UtcNow));
                await SaveAsync(data);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<List<int>> GetRecentIdsAsync(string entityName, int count)
        {
            await _lock.WaitAsync();
            try
            {
                var data = await LoadAsync();
                if (data.TryGetValue(entityName, out var list))
                {
                    return list.OrderByDescending(e => e.Date).Take(count).Select(e => e.Id).ToList();
                }
                return new();
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
