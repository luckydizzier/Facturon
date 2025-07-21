using System.IO;
using Serilog;

namespace Facturon.Startup
{
    public static class LoggingConfiguration
    {
        public static void Configure(LoggerConfiguration configuration)
        {
            Directory.CreateDirectory("logs");

            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.File(
                    Path.Combine("logs", "facturon-.log"),
                    rollingInterval: RollingInterval.Day,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true);
        }
    }
}
