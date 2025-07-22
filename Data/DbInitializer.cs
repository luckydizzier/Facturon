using System;
using System.Linq;
using System.Threading.Tasks;
using Facturon.Data.Initialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Facturon.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services, ILogger logger)
        {
            logger.LogInformation("Starting database initialization...");

            try
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FacturonDbContext>();

                logger.LogInformation("Checking for pending migrations...");
                if (db.Database.GetMigrations().Any())
                {
                    logger.LogInformation("Running EF Core migrations...");
                    await db.Database.MigrateAsync();
                }
                else
                {
                    logger.LogInformation("No migrations found. Ensuring database created...");
                    await db.Database.EnsureCreatedAsync();
                }

                logger.LogInformation("Database ready.");

                if (!await db.Suppliers.AnyAsync())
                {
                    logger.LogInformation("Database is empty. Running seed data...");
                    await SeedData.InitializeAsync(db);
                }

                logger.LogInformation("Database initialization complete.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed.");
                throw new InvalidOperationException("Database migration failed. Please check logs.");
            }
        }
    }
}

