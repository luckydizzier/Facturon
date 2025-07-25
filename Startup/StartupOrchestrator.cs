using System;
using System.IO;
using System.Threading.Tasks;
using Facturon.Data;
using Facturon.Repositories;
using Facturon.Services;
using Facturon.Domain.Entities;
using Facturon.App.ViewModels;
using Facturon.App.Views;
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
                    var baseDir = AppContext.BaseDirectory;
                    var configPath = Path.Combine(baseDir, "appsettings.json");

                    if (!File.Exists(configPath))
                    {
                        var defaultJson = "{\n  \"ConnectionStrings\": { \"Default\": \"" +
                                          $"Data Source={Path.Combine(baseDir, "facturon.db")}" +
                                          "\" }\n}";
                        File.WriteAllText(configPath, defaultJson);
                    }

                    config.AddJsonFile(configPath, optional: false, reloadOnChange: true);
                })
                .UseSerilog((context, services, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    var connectionString = context.Configuration.GetConnectionString("Default") ?? DbPathHelper.GetConnectionString();
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

                    services.AddSingleton<IConfirmationDialogService, ConfirmationDialogService>();
                    services.AddSingleton<INewEntityDialogService<Supplier>, NewSupplierDialogService>();
                    services.AddSingleton<INewEntityDialogService<Product>, NewProductDialogService>();
                    services.AddSingleton<INewEntityDialogService<Unit>, NewUnitDialogService>();
                    services.AddSingleton<INewEntityDialogService<TaxRate>, NewTaxRateDialogService>();
                    services.AddSingleton<INewEntityDialogService<ProductGroup>, NewProductGroupDialogService>();
                    services.AddSingleton<INewEntityDialogService<PaymentMethod>, NewPaymentMethodDialogService>();

                    services.AddSingleton<INavigationService, NavigationService>();

                    services.AddSingleton<MainWindow>();
                    services.AddTransient<InvoiceListViewModel>();
                    services.AddTransient<InvoiceDetailViewModel>(sp =>
                        new InvoiceDetailViewModel(
                            sp.GetRequiredService<IInvoiceService>(),
                            sp.GetRequiredService<IPaymentMethodService>(),
                            sp.GetRequiredService<IProductService>(),
                            sp.GetRequiredService<IUnitService>(),
                            sp.GetRequiredService<ITaxRateService>(),
                            sp.GetRequiredService<ISupplierService>(),
                            sp.GetRequiredService<IInvoiceItemService>(),
                            sp.GetRequiredService<IConfirmationDialogService>(),
                            sp.GetRequiredService<INewEntityDialogService<PaymentMethod>>(),
                            sp.GetRequiredService<INewEntityDialogService<Product>>(),
                            sp.GetRequiredService<INewEntityDialogService<Unit>>(),
                            sp.GetRequiredService<INewEntityDialogService<TaxRate>>(),
                            sp.GetRequiredService<INewEntityDialogService<Supplier>>(),
                            sp.GetRequiredService<INavigationService>(),
                            sp.GetRequiredService<MainViewModel>()));
                    services.AddTransient<MainViewModel>();
                });

            return builder.Build();
        }

        public static async Task StartAsync(IHost host)
        {
            var logger = host.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
            logger.LogInformation("Application starting");

            using var scope = host.Services.CreateScope();
            await DbInitializer.InitializeAsync(scope.ServiceProvider, logger);

            await host.StartAsync();
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
