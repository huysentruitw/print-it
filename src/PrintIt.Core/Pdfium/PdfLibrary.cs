using System;

namespace PrintIt.Core.Pdfium
{
    public sealed class PdfLibrary : IDisposable
    {
        private static readonly object SyncRoot = new object();

#pragma warning disable IDE1006 // Naming Styles

        // ReSharper disable once InconsistentNaming
        private static PdfLibrary _library;
#pragma warning restore IDE1006 // Naming Styles

        public static void EnsureInitialized()
        {
            lock (SyncRoot)
            {
                _library ??= new PdfLibrary();
            }
        }

        private bool _disposed;

        private PdfLibrary()
        {
            NativeMethods.Init();
        }

        ~PdfLibrary()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            NativeMethods.Destroy();
            _disposed = true;
        }
    }
}
