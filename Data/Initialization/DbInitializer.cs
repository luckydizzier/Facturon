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
            try
            {
                Directory.CreateDirectory("logs");
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to ensure logs directory");
            }

            await using var scope = services.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<FacturonDbContext>();
            var connectionString = context.Database.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No database connection string configured.");

            var dbPath = new SqliteConnectionStringBuilder(connectionString).DataSource;
            var dbExists = File.Exists(dbPath);

            if (!dbExists)
            {
                logger.LogInformation("Database file not found. Creating new database at {DbPath}", dbPath);
                await context.Database.MigrateAsync();
                await SeedIfNeededAsync(context, logger);
                return;
            }

            logger.LogInformation("Database found at {DbPath}. Validating schema...", dbPath);

            var missing = await GetMissingTablesAsync(connectionString);
            if (missing.Count > 0)
            {
                logger.LogWarning("Missing tables detected: {Tables}. Attempting migration...", string.Join(", ", missing));
                await context.Database.MigrateAsync();

                missing = await GetMissingTablesAsync(connectionString);
                if (missing.Count > 0)
                {
                    logger.LogError("Database schema is invalid after migration. Missing tables: {Tables}. Consider deleting the database file to recreate it.", string.Join(", ", missing));
                    return;
                }

                logger.LogInformation("Database schema repaired via migration");
            }

            logger.LogInformation("Database schema validated");
            await SeedIfNeededAsync(context, logger);
        }

        private static async Task<List<string>> GetMissingTablesAsync(string connectionString)
        {
            var missing = new List<string>();
            await using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();
            foreach (var table in RequiredTables)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=$name";
                command.Parameters.AddWithValue("$name", table);
                var result = await command.ExecuteScalarAsync();
                if (result == null)
                    missing.Add(table);
            }
            await connection.CloseAsync();
            return missing;
        }

        private static async Task SeedIfNeededAsync(FacturonDbContext context, ILogger logger)
        {
            try
            {
                if (!await context.Suppliers.AnyAsync())
                {
                    logger.LogInformation("Seeding initial data");
                    await SeedData.InitializeAsync(context);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to seed initial data");
            }
        }
    }
}
