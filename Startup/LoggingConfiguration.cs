using System;
using System.IO;
using Serilog;

namespace Facturon.App
{
    public static class LoggingConfiguration
    {
        public static void Configure(LoggerConfiguration configuration)
        {
            var logDir = Path.Combine(AppContext.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);

            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    Path.Combine(logDir, "facturon-.log"),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true);
        }
    }
}
