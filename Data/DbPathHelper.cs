using System;
using System.IO;

namespace Facturon.Data
{
    public static class DbPathHelper
    {
        public static string GetConnectionString()
        {
            var baseDir = AppContext.BaseDirectory;
            var dbPath = Path.Combine(baseDir, "facturon.db");
            return $"Data Source={dbPath}";
        }
    }
}
