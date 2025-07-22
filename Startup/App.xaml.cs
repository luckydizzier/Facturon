using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Facturon.App.Views;
using Facturon.App.ViewModels;
using Microsoft.Extensions.Logging;

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

            var logger = Host.Services.GetRequiredService<ILoggerFactory>().CreateLogger<App>();
            logger.LogInformation("Starting UI");

            var window = Host.Services.GetRequiredService<MainWindow>();
            var vm = Host.Services.GetRequiredService<MainViewModel>();
            vm.InitializeAsync().GetAwaiter().GetResult();

            window.DataContext = vm;
            MainWindow = window;
            window.Show();
            logger.LogInformation("MainWindow displayed");
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
