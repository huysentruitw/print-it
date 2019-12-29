using System;
using System.IO;
using System.Runtime.InteropServices;

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
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            byte[] buffer = memoryStream.ToArray();
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            NativeMethods.DocumentHandle documentHandle = NativeMethods.LoadDocument(handle.AddrOfPinnedObject(), buffer.Length, password);
            return new InMemoryPdfDocument(documentHandle, handle);
        }

        public virtual void Dispose()
        {
            _documentHandle.Close();
            _documentHandle.Dispose();
        }

        public int PageCount => NativeMethods.GetPageCount(_documentHandle);

        public PdfPage OpenPage(int pageIndex)
        {
            NativeMethods.PageHandle pageHandle = NativeMethods.LoadPage(_documentHandle, pageIndex);

            if (pageHandle.IsInvalid)
            {
                throw new IndexOutOfRangeException($"Failed to open page with index {pageIndex}");
            }

            return new PdfPage(this, pageHandle);
        }
    }
}
