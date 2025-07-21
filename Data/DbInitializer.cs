using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Facturon.Data
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
            logger.LogInformation("Starting database initialization...");

            await using var scope = services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<FacturonDbContext>();
            var connectionString = db.Database.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("No database connection string configured.");

            var dbPath = new SqliteConnectionStringBuilder(connectionString).DataSource;
            logger.LogInformation("Database file found at {Path}", dbPath);
            var dbExists = File.Exists(dbPath);
            try
            {
                if (!dbExists)
                {
                    logger.LogInformation("Database file missing, creating new database...");
                    logger.LogInformation("Running EF Core migrations...");
                    await db.Database.MigrateAsync();
                }
                else
                {
                    await using var connection = new SqliteConnection(connectionString);
                    await connection.OpenAsync();

                    var hasTables = await AnyTablesAsync(connection);
                    if (!hasTables)
                    {
                        logger.LogWarning("No tables found. Running initial migration...");
                        await db.Database.MigrateAsync();
                    }
                    else
                    {
                        var missing = await GetMissingTablesAsync(connection);
                        if (missing.Count > 0)
                        {
                            logger.LogError("SQLite schema check failed: missing tables {Tables}", string.Join(", ", missing));
                            throw new InvalidOperationException("Database schema invalid or incomplete. Please recreate or repair the database.");
                        }
                    }
                }

                logger.LogInformation("Database ready.");

                if (!await db.Suppliers.AnyAsync())
                {
                    var seedMethod = typeof(Initialization.SeedData).GetMethod("SeedAsync");
                    if (seedMethod != null)
                    {
                        await (Task)seedMethod.Invoke(null, new object?[] { db, logger })!;
                    }
                    else if (typeof(Initialization.SeedData).GetMethod("InitializeAsync") != null)
                    {
                        await Initialization.SeedData.InitializeAsync(db);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "SQLite schema check failed: {Error}", ex.Message);
                throw;
            }
            finally
            {
                await db.DisposeAsync();
            }
        }

        private static async Task<bool> AnyTablesAsync(SqliteConnection connection)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' LIMIT 1";
            var result = await command.ExecuteScalarAsync();
            return result != null;
        }

        private static async Task<List<string>> GetMissingTablesAsync(SqliteConnection connection)
        {
            var missing = new List<string>();
            foreach (var table in RequiredTables)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name=$name";
                command.Parameters.AddWithValue("$name", table);
                var result = await command.ExecuteScalarAsync();
                if (result == null)
                    missing.Add(table);
            }
            return missing;
        }
    }
}
