using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace Facturon.App
{
    public partial class App : Application
    {
        public IHost? Host { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var host = Host;
            if (host != null)
            {
                StartupOrchestrator.Start(host);
            }
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
