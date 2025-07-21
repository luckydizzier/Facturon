using System;
using System.Windows;
using Microsoft.Extensions.Hosting;

namespace Facturon.App
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var host = StartupOrchestrator.BuildHost(args);

            var app = new App { Host = host };
            app.InitializeComponent();
            app.Run();
        }
    }
}
