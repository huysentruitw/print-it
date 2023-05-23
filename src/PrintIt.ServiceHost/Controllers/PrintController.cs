using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net.Http;
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
                numberOfCopies: request.Copies ?? 1);
            return Ok();
        }

        [HttpPost]
        [Route("from-base64")]
        public async Task<IActionResult> PrintFromBase64Pdf([FromBody] PrintFromBas64Request request)
        {
            var bytes = Convert.FromBase64String(request.PdfContent);            
            await using Stream pdfStream = new MemoryStream(bytes);
            _pdfPrintService.Print(pdfStream,
                printerName: request.PrinterPath,
                pageRange: request.PageRange,
                numberOfCopies: request.Copies ?? 1);
            return Ok();
        }
    }

    public class BasePrintRequest
    {
        [Required]
        public string PrinterPath { get; set; }

        public string PageRange { get; set; }

        public int? Copies { get; set; }
    }

    public sealed class PrintFromTemplateRequest : BasePrintRequest
    {
        [Required]
        public IFormFile PdfFile { get; set; }        
    }

    public sealed class PrintFromBas64Request : BasePrintRequest
    {
        [Required]
        public string PdfContent { get; set; }
    }
}
