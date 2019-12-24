using Microsoft.Extensions.DependencyInjection;

namespace PrintIt.Core
{
    public static class PrintItServiceCollectionExtensions
    {
        public static IServiceCollection AddPrintIt(this IServiceCollection services)
        {
            services.AddSingleton<IPrinterService, PrinterService>();
            return services;
        }
    }
}
