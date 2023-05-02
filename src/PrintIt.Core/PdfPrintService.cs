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

        public void Print(Stream pdfStream, string printerName, string pageRange = null, int numberOfCopies = 1, string paperSource = null, string paperSize = null)
        {
            if (pdfStream == null)
                throw new ArgumentNullException(nameof(pdfStream));

            PdfDocument document = PdfDocument.Open(pdfStream);

            _logger.LogInformation($"Printing PDF containing {document.PageCount} page(s) to printer '{printerName}'");

            using var printDocument = new PrintDocument();
            printDocument.PrinterSettings.PrinterName = printerName;
            printDocument.PrinterSettings.Copies = (short)Math.Clamp(numberOfCopies, 1, short.MaxValue);

            PaperSource chosenSource = null;
            if (paperSource != null)
            {
                foreach (PaperSource source in printDocument.PrinterSettings.PaperSources)
                {
                    if (source != null && source.SourceName == paperSource)
                    {
                        printDocument.PrinterSettings.DefaultPageSettings.PaperSource = source;
                        chosenSource = source;
                        break;
                    }
                }

                if (chosenSource == null)
                {
                    throw new SelectPaperSourceException(paperSource, printerName);
                }
            }

            if (paperSize != null)
            {
                bool paperSizeSet = false;
                foreach (PaperSize size in printDocument.PrinterSettings.PaperSizes)
                {
                    if (size != null && size.PaperName == paperSize)
                    {
                        printDocument.DefaultPageSettings.PaperSize = size;
                        paperSizeSet = true;
                        break;
                    }
                }

                if (!paperSizeSet)
                {
                    throw new SelectPaperSizeException(paperSize, printerName);
                }
            }

            PrintState state = PrintStateFactory.Create(document, pageRange);
            printDocument.PrintPage += (_, e) => PrintDocumentOnPrintPage(e, state);
            printDocument.QueryPageSettings += (_, e) => MyPrintQueryPageSettingsEvent(e, chosenSource);
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

        private void MyPrintQueryPageSettingsEvent(QueryPageSettingsEventArgs e, PaperSource paperSource)
        {
            if (paperSource != null)
            {
                e.PageSettings.PaperSource = paperSource;
            }
        }
    }

    public interface IPdfPrintService
    {
        void Print(Stream pdfStream, string printerName, string pageRange = null, int numberOfCopies = 1, string paperSource = null, string paperSize = null);
    }

    public sealed class SelectPaperSizeException : Exception
    {
        public SelectPaperSizeException(string paperSize, string printerPath)
            : base($"PaperSize: {paperSize} was not valid for printerName: {printerPath}")
        {
            PrinterPath = printerPath;
            PaperSize = paperSize;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
        public string PrinterPath { get; }

        public string PaperSize { get; }
    }

    public sealed class SelectPaperSourceException : Exception
    {
        public SelectPaperSourceException(string paperSource, string printerPath)
            : base($"PaperSource: {paperSource} was not valid for PrinterName: {printerPath}")
        {
            PrinterPath = printerPath;
            PaperSource = paperSource;
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Public API")]
        public string PrinterPath { get; }

        public string PaperSource { get; }
    }
}
