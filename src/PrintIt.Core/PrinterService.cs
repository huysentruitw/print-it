using System.Drawing.Printing;
using System.Linq;

namespace PrintIt.Core
{
    internal sealed class PrinterService : IPrinterService
    {
        public string[] GetInstalledPrinters()
            => PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
    }

    public interface IPrinterService
    {
        string[] GetInstalledPrinters();
    }
}
