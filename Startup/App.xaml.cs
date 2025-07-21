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
            if (Host != null)
            {
                StartupOrchestrator.Start(Host);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Host != null)
            {
                StartupOrchestrator.StopAsync(Host).GetAwaiter().GetResult();
            }
            base.OnExit(e);
        }
    }
}
