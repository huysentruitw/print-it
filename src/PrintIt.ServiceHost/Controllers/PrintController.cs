using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintIt.Core;

namespace PrintIt.ServiceHost.Controllers
{
    [ApiController]
    [Route("print")]
    public class PrintController : ControllerBase
    {
        private readonly IPdfPrintService _pdfPrintService;

        public PrintController(IPdfPrintService pdfPrintService)
        {
            _pdfPrintService = pdfPrintService;
        }

        [HttpPost]
        [Route("from-pdf")]
        public async Task<IActionResult> PrintFromPdf([FromForm] PrintFromTemplateRequest request)
        {
            await using Stream pdfStream = request.PdfFile.OpenReadStream();
            _pdfPrintService.Print(pdfStream,
                printerName: request.PrinterPath,
                pageRange: request.PageRange,
                numberOfCopies: request.Copies ?? 1,
                paperSource: request.PaperSource, 
                paperSize: request.PaperSize);
            return Ok();
        }
    }

    public sealed class PrintFromTemplateRequest
    {
        [Required]
        public IFormFile PdfFile { get; set; }

        [Required]
        public string PrinterPath { get; set; }

        public string PageRange { get; set; }

        public int? Copies { get; set; }

        public string PaperSource { get; set; }

        public string PaperSize { get; set; }
    }
}
