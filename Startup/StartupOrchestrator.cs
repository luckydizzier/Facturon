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

namespace Facturon.App
{
    public static class StartupOrchestrator
    {
        public static IHost BuildHost(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(AppContext.BaseDirectory);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .UseSerilog((context, services, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("Default");
                    services.AddDbContext<FacturonDbContext>(options => options.UseSqlite(connectionString));

                    services.AddScoped<IInvoiceRepository, EfInvoiceRepository>();
                    services.AddScoped<IPaymentMethodRepository, EfPaymentMethodRepository>();
                    services.AddScoped<IProductGroupRepository, EfProductGroupRepository>();
                    services.AddScoped<IProductRepository, EfProductRepository>();
                    services.AddScoped<ISupplierRepository, EfSupplierRepository>();
                    services.AddScoped<ITaxRateRepository, EfTaxRateRepository>();
                    services.AddScoped<IUnitRepository, EfUnitRepository>();

                    services.AddScoped<IInvoiceService, InvoiceService>();
                    services.AddScoped<IInvoiceItemService, InvoiceItemService>();
                    services.AddScoped<IPaymentMethodService, PaymentMethodService>();
                    services.AddScoped<IProductGroupService, ProductGroupService>();
                    services.AddScoped<IProductService, ProductService>();
                    services.AddScoped<ISupplierService, SupplierService>();
                    services.AddScoped<ITaxRateService, TaxRateService>();
                    services.AddScoped<IUnitService, UnitService>();
                });

            return builder.Build();
        }

        public static void Start(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
            logger.LogInformation("Application starting");

            var context = host.Services.GetRequiredService<FacturonDbContext>();
            var dbPath = new SqliteConnectionStringBuilder(context.Database.GetConnectionString()).DataSource;
            logger.LogInformation("Database path: {DbPath}", dbPath);
            if (!File.Exists(dbPath))
            {
                logger.LogInformation("Creating database at {DbPath}", dbPath);
                context.Database.EnsureCreated();
            }

            host.Start();
        }

        public static async Task StopAsync(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Shutdown");
            logger.LogInformation("Application shutting down");

            await host.StopAsync();
            host.Dispose();
            Log.CloseAndFlush();
        }
    }
}
