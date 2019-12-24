using System;
using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.ServiceHost.Controllers
{
    [ApiController]
    [Route("api/printers")]
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
            var installedPrinters = _printerService.GetInstalledPrinters(); 
            return Ok(installedPrinters);
        }
    }
}
