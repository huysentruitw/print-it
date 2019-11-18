using System;

namespace PrintIt.Core.Pdfium
{
    public sealed class MemoryBasedPdfDocument : PdfDocument
    {
        private readonly byte[] _buffer;

        internal MemoryBasedPdfDocument(NativeMethods.DocumentHandle documentHandle, byte[] buffer)
            : base(documentHandle)
        {
            _buffer = buffer;
        }
    }
}
