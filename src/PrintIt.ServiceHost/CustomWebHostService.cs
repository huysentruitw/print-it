using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PrintIt.ServiceHost
{
    [ExcludeFromCodeCoverage]
    internal sealed class CustomWebHostService : WebHostService
    {
        private readonly ILogger _logger;

        public CustomWebHostService(IWebHost host)
            : base(host)
        {
            _logger = host.Services.GetRequiredService<ILogger<CustomWebHostService>>();
        }

        protected override void OnStarting(string[] args)
        {
            _logger.LogInformation("PrintIt Starting...");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("PrintIt Started.");
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("PrintIt Stopping...");
            base.OnStopping();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("PrintIt Stopped.");
            base.OnStopped();
        }
    }
}
