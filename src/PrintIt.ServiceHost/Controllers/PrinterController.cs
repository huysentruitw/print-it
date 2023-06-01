using System.Collections.Generic;
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

        [HttpGet]
        [Route("paperSources")]
        public IActionResult ListPaperSources([FromQuery] string printerPath)
        {
            string[] paperSources = _printerService.GetPrinterPageSources(printerPath);
            return Ok(paperSources);
        }

        [HttpGet]
        [Route("paperSizes")]
        public IActionResult ListPaperSizes([FromQuery] string printerPath)
        {
            string[] paperSizes = _printerService.GetPrinterPageSizes(printerPath);
            return Ok(paperSizes);
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
