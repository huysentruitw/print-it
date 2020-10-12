using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PrintIt.Core.Pdfium;

namespace PrintIt.ServiceHost
{
    [ExcludeFromCodeCoverage]
    public static class Program
    {
        public static void Main(string[] args)
        {
            PdfLibrary.EnsureInitialized();

            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule?.FileName ?? string.Empty;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                Directory.SetCurrentDirectory(pathToContentRoot);
            }

            IWebHost host = CreateWebHostBuilder(args.Where(arg => arg != "--console").ToArray(), isService).Build();

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

        private static IWebHostBuilder CreateWebHostBuilder(string[] args, bool isService)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging
                        .ClearProviders()
                        .AddFilter("Microsoft", LogLevel.Warning)
                        .AddFilter("Microsoft.AspNetCore.Mvc.Internal", LogLevel.Warning)
                        .AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Warning)
                        .AddFilter("System", LogLevel.Warning)
                        .AddConsole();

                    if (isService)
                        logging.AddEventLog(settings =>
                        {
                            settings.SourceName = "PrintIt";
                        });
                })
                .UseStartup<Startup>()
                .UseUrls(configuration.GetValue<string>("Host:Urls"))
                .UseKestrel()
                .UseConfiguration(configuration);
        }
    }
}
