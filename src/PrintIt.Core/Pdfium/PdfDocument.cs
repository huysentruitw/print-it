using System;
using System.IO;

namespace PrintIt.Core.Pdfium
{
    public abstract class PdfDocument : IDisposable
    {
        private readonly NativeMethods.DocumentHandle _documentHandle;

        internal PdfDocument(NativeMethods.DocumentHandle documentHandle)
        {
            _documentHandle = documentHandle;
        }

        public static InMemoryPdfDocument Open(Stream stream, string password = null)
        {
            password = password ?? string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var buffer = memoryStream.ToArray();
                var documentHandle = NativeMethods.LoadDocument(buffer, buffer.Length, password);
                return new InMemoryPdfDocument(documentHandle, buffer);
            }
        }
        
        public virtual void Dispose()
        {
            _documentHandle.Dispose();
        }

        public int PageCount => NativeMethods.GetPageCount(_documentHandle);

        public PdfPage OpenPage(int pageIndex)
        {
            var pageHandle = NativeMethods.LoadPage(_documentHandle, pageIndex);

            if (pageHandle.IsInvalid)
            {
                throw new IndexOutOfRangeException($"Failed to open page with index {pageIndex}");
            }

            return new PdfPage(this, pageHandle);
        }
    }
}
