using System;
using System.Drawing;

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

        public void Dispose()
        {
            _pageHandle.Dispose();
        }
        
        public PdfDocument Document { get; }

        public SizeF SizeInInch
        {
            get
            {
                float widthInPoints = NativeMethods.GetPageWidth(_pageHandle);
                float heightInPoints = NativeMethods.GetPageHeight(_pageHandle);
                return new SizeF(widthInPoints / 72.0f, heightInPoints / 72.0f);
            }
        }
    }
}
