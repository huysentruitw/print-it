using System;
using System.Runtime.InteropServices;

namespace PrintIt.Core.Pdfium
{
    public sealed class InMemoryPdfDocument : PdfDocument
    {
        private readonly GCHandle _handle;

        internal InMemoryPdfDocument(NativeMethods.DocumentHandle documentHandle, GCHandle handle)
            : base(documentHandle)
        {
            _handle = handle;
        }

        public override void Dispose()
        {
            base.Dispose();

            _handle.Free();
            GC.SuppressFinalize(true);
        }
    }
}
