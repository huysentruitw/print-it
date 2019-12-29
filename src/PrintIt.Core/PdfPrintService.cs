using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using Microsoft.Extensions.Logging;
using PrintIt.Core.Pdfium;

namespace PrintIt.Core
{
    internal sealed class PdfPrintService : IPdfPrintService
    {
        private readonly ILogger<PdfPrintService> _logger;

        public PdfPrintService(ILogger<PdfPrintService> logger)
        {
            _logger = logger;
        }

        public void Print(Stream pdfStream, string printerName)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            PdfDocument document = PdfDocument.Open(pdfStream);

            _logger.LogInformation($"Printing PDF from stream containing {document.PageCount} page(s)");

            using var printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = printerName;
            var state = new PrintState(document);
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

        private sealed class PrintState
        {
            private readonly int _pageCount;

            public PrintState(PdfDocument document)
            {
                Document = document;
                _pageCount = document.PageCount;
                CurrentPageIndex = 0;
            }

            public PdfDocument Document { get; }

            public int CurrentPageIndex { get; private set; }

            public bool AdvanceToNextPage()
            {
                if (CurrentPageIndex >= _pageCount)
                    return false;

                CurrentPageIndex++;
                return CurrentPageIndex < _pageCount;
            }
        }
    }

    public interface IPdfPrintService
    {
        void Print(Stream pdfStream, string printerName);
    }
}
