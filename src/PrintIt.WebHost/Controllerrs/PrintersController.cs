using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.WebHost.Controllerrs
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrintersController : ControllerBase
    {

        private readonly IPrinterService _printerService;

        public PrintersController(IPrinterService printerService)
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
