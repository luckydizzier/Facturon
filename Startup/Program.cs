using System;
using System.IO;
using System.Threading.Tasks;
using Facturon.Data;
using Facturon.Repositories;
using Facturon.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Facturon.Startup
{
    public static class Program
    {
        [STAThread]
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("Startup/appsettings.json", optional: true, reloadOnChange: true);
                })
                .UseSerilog((context, services, configuration) =>
                {
                    LoggingConfiguration.Configure(configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("Default");
                    services.AddDbContext<FacturonDbContext>(options =>
                        options.UseSqlite(connectionString));

                    // Repositories
                    services.AddScoped<IInvoiceRepository, EfInvoiceRepository>();
                    services.AddScoped<IPaymentMethodRepository, EfPaymentMethodRepository>();
                    services.AddScoped<IProductGroupRepository, EfProductGroupRepository>();
                    services.AddScoped<IProductRepository, EfProductRepository>();
                    services.AddScoped<ISupplierRepository, EfSupplierRepository>();
                    services.AddScoped<ITaxRateRepository, EfTaxRateRepository>();
                    services.AddScoped<IUnitRepository, EfUnitRepository>();

                    // Services
                    services.AddScoped<IInvoiceService, InvoiceService>();
                    services.AddScoped<IInvoiceItemService, InvoiceItemService>();
                    services.AddScoped<IPaymentMethodService, PaymentMethodService>();
                    services.AddScoped<IProductGroupService, ProductGroupService>();
                    services.AddScoped<IProductService, ProductService>();
                    services.AddScoped<ISupplierService, SupplierService>();
                    services.AddScoped<ITaxRateService, TaxRateService>();
                    services.AddScoped<IUnitService, UnitService>();
                });

            var host = builder.Build();

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            try
            {
                logger.LogInformation("Application starting");

                var context = host.Services.GetRequiredService<FacturonDbContext>();
                var dbPath = new SqliteConnectionStringBuilder(context.Database.GetConnectionString()).DataSource;
                logger.LogInformation("Database path: {DbPath}", dbPath);
                logger.LogInformation("Database file exists: {Exists}", File.Exists(dbPath));

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                logger.LogInformation("Application shutting down");
                Log.CloseAndFlush();
            }
        }
    }
}
