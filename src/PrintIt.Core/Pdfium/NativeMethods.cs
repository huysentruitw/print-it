using System;
using System.Drawing;
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
                Imports.FPDF_InitLibrary();
            }
        }

        public static void Destroy()
        {
            lock (SyncRoot)
            {
                Imports.FPDF_DestroyLibrary();
            }
        }

        public static DocumentHandle LoadDocument(IntPtr buffer, int size, string password)
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

        public static void RenderPage(PageHandle pageHandle, IntPtr hdc, Rectangle boundingBox,
            PageOrientation pageOrientation, PageRenderingFlags flags)
        {
            lock (SyncRoot)
            {
                Imports.FPDF_RenderPage(hdc, pageHandle,
                    startX: boundingBox.Left, startY: boundingBox.Top,
                    sizeX: boundingBox.Width, sizeY: boundingBox.Height,
                    pageOrientation: (int)pageOrientation, flags: (int)flags);
            }
        }

        public enum PageOrientation : int
        {
            Normal = 0,
            Rotated90DegreesClockwise = 1,
            Rotated180Degrees = 2,
            Rotated90DegreesCounterClockwise = 3,
        }

        [Flags]
        public enum PageRenderingFlags : int
        {
            RenderAnnotations = 0x1,
            OptimizeForLcdDisplay = 0x2,
            DontUseNativeTextOutput = 0x4,
            Grayscale = 0x8,
            BitmapsInReverseByteOrder = 0x10,
            LimitImageCacheSize = 0x200,
            ForceHalftoneForImageStretching = 0x400,
            OptimizeForPrinting = 0x800,
            DisableTextAntiAliasing = 0x1000,
            DisableImageAntiAliasing = 0x2000,
            DisablePathAntiAliasing = 0x4000,
        }

        [SecurityCritical]
        public sealed class DocumentHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public DocumentHandle()
                : base(true)
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
        public sealed class PageHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public PageHandle()
                : base(true)
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
            public static extern DocumentHandle FPDF_LoadMemDocument(IntPtr buffer, int size, string password);

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
            public static extern void FPDF_RenderPage(IntPtr hdc, PageHandle pageHandle, int startX, int startY,
                int sizeX, int sizeY, int pageOrientation, int flags);

            [DllImport(DllName, CallingConvention = PdfiumCallingConvention)]
            public static extern void FPDF_ClosePage(IntPtr pageHandle);
        }
    }
}
