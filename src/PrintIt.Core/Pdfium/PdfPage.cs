using System;

namespace PrintIt.Core.Pdfium
{
    public sealed class PdfPage : IDisposable
    {
        private readonly NativeMethods.PageHandle _pageHandle;

        internal PdfPage(PdfDocument document, NativeMethods.PageHandle pageHandle)
        {
            Document = document;
            _pageHandle = pageHandle;
        }
        
        public PdfDocument Document { get; }

        public void Dispose()
        {
            _pageHandle.Dispose();
        }
    }
}
