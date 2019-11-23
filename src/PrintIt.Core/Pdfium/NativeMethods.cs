using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace PrintIt.Core.Pdfium
{
    internal static class NativeMethods
    {
        // Cross-appdomain SyncRoot
        private static readonly string SyncRoot = string.Intern("thq@rCP%WL$K5&61C^DcSpJ6oue79uyB");

        public static void Init()
        {
            lock (SyncRoot)
            {
                Imports.FPDF_InitLibrary();;
            }
        }

        public static void Destroy()
        {
            lock (SyncRoot)
            {
                Imports.FPDF_DestroyLibrary();
            }
        }
        
        public static DocumentHandle LoadDocument(byte[] buffer, int size, string password)
        {
            lock (SyncRoot)
            {
                return Imports.FPDF_LoadMemDocument(buffer, size, password ?? string.Empty);
            }
        }

        public static int GetPageCount(DocumentHandle documentHandle)
        {
            lock (SyncRoot)
            {
                return Imports.FPDF_GetPageCount(documentHandle);
            }
        }

        public static PageHandle LoadPage(DocumentHandle documentHandle, int pageIndex)
        {
            lock (SyncRoot)
            {
                return Imports.FPDF_LoadPage(documentHandle, pageIndex);
            }
        }

        public static float GetPageWidth(PageHandle pageHandle)
        {
            lock (SyncRoot)
            {
                return Imports.FPDF_GetPageWidthF(pageHandle);
            }
        }

        public static float GetPageHeight(PageHandle pageHandle)
        {
            lock (SyncRoot)
            {
                return Imports.FPDF_GetPageHeightF(pageHandle);
            }
        }
        
        [SecurityCritical]
        // ReSharper disable once ClassNeverInstantiated.Global
        public sealed class DocumentHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public DocumentHandle() : base(true)
            {
            }

            [SecurityCritical]
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            protected override bool ReleaseHandle()
            {
                lock (SyncRoot)
                {
                    Imports.FPDF_CloseDocument(handle);
                }
                
                return true;
            }
        }

        [SecurityCritical]
        // ReSharper disable once ClassNeverInstantiated.Global
        public sealed class PageHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public PageHandle() : base(true)
            {
            }
            
            [SecurityCritical]
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            protected override bool ReleaseHandle()
            {
                lock (SyncRoot)
                {
                    Imports.FPDF_ClosePage(handle);
                }
                
                return true;
            }
        }
        
        [SuppressMessage("ReSharper", "IdentifierTypo")]
        private static class Imports
        {
            private const string DllName = "pdfium.dll";
            private const CharSet PdfiumCharSet = CharSet.Ansi;
            private const CallingConvention PdfiumCallingConvention = CallingConvention.StdCall;
            
            [DllImport(DllName)]
            public static extern void FPDF_InitLibrary();

            [DllImport(DllName)]
            public static extern void FPDF_DestroyLibrary();
            
            [DllImport(DllName, CallingConvention = PdfiumCallingConvention, CharSet = PdfiumCharSet)]
            public static extern DocumentHandle FPDF_LoadMemDocument(byte[] buffer, int size, string password);
            
            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern int FPDF_GetPageCount(DocumentHandle documentHandle);

            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern PageHandle FPDF_LoadPage(DocumentHandle documentHandle, int pageIndex);
            
            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern void FPDF_CloseDocument(IntPtr documentHandle);

            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern float FPDF_GetPageWidthF(PageHandle pageHandle);
            
            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern float FPDF_GetPageHeightF(PageHandle pageHandle);
            
            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern void FPDF_ClosePage(IntPtr pageHandle);
        }
    }
}
