using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Facturon.App.Views;
using Facturon.App.ViewModels;
using Facturon.Services;

namespace Facturon.App
{
    public partial class App : Application
    {
        public IHost? Host { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Host = StartupOrchestrator.BuildHost(e.Args);
            StartupOrchestrator.StartAsync(Host).GetAwaiter().GetResult();

            var invoiceService = Host.Services.GetRequiredService<IInvoiceService>();
            var vm = new MainViewModel(invoiceService);
            vm.InitializeAsync().GetAwaiter().GetResult();
            var window = new MainWindow { DataContext = vm };
            MainWindow = window;
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var host = Host;
            if (host != null)
            {
                StartupOrchestrator.StopAsync(host).GetAwaiter().GetResult();
            }
            base.OnExit(e);
        }
    }
}
