using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace PrintIt.Core
{
    [ExcludeFromCodeCoverage]
    public static class PrintItServiceCollectionExtensions
    {
        public static IServiceCollection AddPrintIt(this IServiceCollection services)
        {
            services.AddSingleton<ICommandService, CommandService>();
            services.AddSingleton<IPdfPrintService, PdfPrintService>();
            services.AddSingleton<IPrinterService, PrinterService>();
            return services;
        }
    }
}
