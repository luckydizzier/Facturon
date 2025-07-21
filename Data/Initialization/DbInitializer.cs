using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Facturon.Data.Initialization
{
    public static class DbInitializer
    {
        private static readonly string[] RequiredTables = new[]
        {
            "Invoices", "InvoiceItems", "Products", "Suppliers",
            "PaymentMethods", "Units", "TaxRates", "ProductGroups"
        };

        public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
        {
            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<FacturonDbContext>();
            var connectionString = context.Database.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No database connection string configured.");

            var dbPath = new SqliteConnectionStringBuilder(connectionString).DataSource;
            var dbExists = File.Exists(dbPath);

            if (!dbExists)
            {
                logger.LogInformation("Creating new database at {DbPath}", dbPath);
                await context.Database.MigrateAsync();
                await SeedData.InitializeAsync(context);
                return;
            }

            logger.LogInformation("Database found at {DbPath}. Validating schema...", dbPath);

            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    existing.Add(reader.GetString(0));
                }
            }
            await connection.CloseAsync();

            var missing = RequiredTables.Where(t => !existing.Contains(t)).ToList();
            if (missing.Count > 0)
            {
                logger.LogWarning("Database schema missing tables: {Tables}", string.Join(", ", missing));
                throw new InvalidOperationException("Database exists but schema is invalid. Recreate or repair it.");
            }

            logger.LogInformation("Database schema OK.");
        }
    }
}
