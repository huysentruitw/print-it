using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.ServiceHost.Controllers
{
    [ApiController]
    [Route("printers")]
    public sealed class PrinterController : ControllerBase
    {
        private readonly IPrinterService _printerService;

        public PrinterController(IPrinterService printerService)
        {
            _printerService = printerService;
        }

        [HttpGet]
        [Route("list")]
        public IActionResult ListPrinters()
        {
            string[] installedPrinters = _printerService.GetInstalledPrinters();
            return Ok(installedPrinters);
        }

        [HttpPost]
        [Route("install")]
        public IActionResult InstallPrinter([FromQuery] string printerPath)
        {
            _printerService.InstallPrinter(printerPath);
            return Ok();
        }
    }
}
