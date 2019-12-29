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
            _pageHandle.Close();
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

        public void RenderTo(Graphics graphics, bool maintainAspectRatio = true,  bool allowUpscaling = false, bool center = true)
        {
            bool rotate = false;
            SizeF pageSize = SizeInInch;

            var pageAspect = pageSize.Width / pageSize.Height;
            var destinationAspect = graphics.VisibleClipBounds.Width / graphics.VisibleClipBounds.Height;

            if ((pageAspect >= 1.0f && destinationAspect < 1.0f) ||
                (pageAspect < 1.0f && destinationAspect >= 1.0f))
            {
                rotate = true;
            }

            var pageWidthInPixels = (rotate ? pageSize.Height : pageSize.Width) * graphics.DpiX;
            var pageHeightInPixels = (rotate ? pageSize.Width : pageSize.Height) * graphics.DpiY;

            var scaleX = graphics.VisibleClipBounds.Width / pageWidthInPixels;
            var scaleY = graphics.VisibleClipBounds.Height / pageHeightInPixels;

            if (!allowUpscaling)
            {
                scaleX = Math.Min(scaleX, 1.0f);
                scaleY = Math.Min(scaleY, 1.0f);
            }

            if (maintainAspectRatio)
            {
                scaleX = scaleY = Math.Min(scaleX, scaleY);
            }

            var destinationWidth = Math.Min(pageWidthInPixels * scaleX, graphics.VisibleClipBounds.Width);
            var destinationHeight = Math.Min(pageHeightInPixels * scaleY, graphics.VisibleClipBounds.Height);

            var offsetX = 0.0f;
            var offsetY = 0.0f;
            if (center)
            {
                offsetX = (graphics.VisibleClipBounds.Width - destinationWidth) / 2.0f;
                offsetY = (graphics.VisibleClipBounds.Height - destinationHeight) / 2.0f;
            }

            var boundingBox = new Rectangle(
                x: (int)(graphics.VisibleClipBounds.X + offsetX),
                y: (int)(graphics.VisibleClipBounds.Y + offsetY),
                width: (int)destinationWidth,
                height: (int)destinationHeight);

            NativeMethods.RenderPage(_pageHandle, graphics.GetHdc(), boundingBox,
                pageOrientation: rotate ? NativeMethods.PageOrientation.Rotated90DegreesCounterClockwise : NativeMethods.PageOrientation.Normal,
                flags: NativeMethods.PageRenderingFlags.OptimizeForPrinting);
        }
    }
}
