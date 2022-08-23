using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Microsoft.Extensions.Logging;
using PrintIt.Core.Internal;
using PrintIt.Core.Pdfium;

namespace PrintIt.Core
{
    [ExcludeFromCodeCoverage]
    internal sealed class PdfPrintService : IPdfPrintService
    {
        private readonly ILogger<PdfPrintService> _logger;

        public PdfPrintService(ILogger<PdfPrintService> logger)
        {
            _logger = logger;
        }

        public void Print(Stream pdfStream, string printerName, string pageRange = null, int numberOfCopies = 1)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            PdfDocument document = PdfDocument.Open(pdfStream);

            _logger.LogInformation($"Printing PDF containing {document.PageCount} page(s) to printer '{printerName}'");

            using var printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.PrinterSettings.Copies = (short)Math.Clamp(numberOfCopies, 1, short.MaxValue);
            PrintState state = PrintStateFactory.Create(document, pageRange);
            printDocument.PrintPage += (_, e) => PrintDocumentOnPrintPage(e, state);
            printDocument.Print();
        }

        private void PrintDocumentOnPrintPage(PrintPageEventArgs e, PrintState state)
        {
            var destinationRect = new RectangleF(
                x: e.Graphics.VisibleClipBounds.X * e.Graphics.DpiX / 100.0f,
                y: e.Graphics.VisibleClipBounds.Y * e.Graphics.DpiY / 100.0f,
                width: e.Graphics.VisibleClipBounds.Width * e.Graphics.DpiX / 100.0f,
                height: e.Graphics.VisibleClipBounds.Height * e.Graphics.DpiY / 100.0f);
            using PdfPage page = state.Document.OpenPage(state.CurrentPageIndex);
            page.RenderTo(e.Graphics, destinationRect);
            e.HasMorePages = state.AdvanceToNextPage();
        }
    }

    public interface IPdfPrintService
    {
        void Print(Stream pdfStream, string printerName, string pageRange = null, int numberOfCopies = 1);
    }
}
