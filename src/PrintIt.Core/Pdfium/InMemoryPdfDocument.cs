using System;

namespace PrintIt.Core.Pdfium
{
    public sealed class InMemoryPdfDocument : PdfDocument
    {
        private readonly byte[] _buffer;

        internal InMemoryPdfDocument(NativeMethods.DocumentHandle documentHandle, byte[] buffer)
            : base(documentHandle)
        {
            _buffer = buffer;
        }
    }
}
