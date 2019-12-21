using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PrintIt.ServiceHost
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            IWebHostBuilder builder = CreateWebHostBuilder(
                args.Where(arg => arg != "--console").ToArray());

            IWebHost host = builder.Build();

            if (isService)
            {
                using var customWebHostService = new CustomWebHostService(host);
                ServiceBase.Run(customWebHostService);
            }
            else
            {
                host.Run();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging
                        .ClearProviders()
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("Microsoft.AspNetCore.Mvc.Internal", LogLevel.Warning)
                        .AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole()
                        .AddEventLog(settings => { settings.SourceName = "PrintIt"; });
                })
                .UseUrls("http://localhost:7001")
                .UseStartup<Startup>();
    }
}
