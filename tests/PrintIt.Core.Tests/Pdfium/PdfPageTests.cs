using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using FluentAssertions;
using PrintIt.Core.Pdfium;
using Xunit;

namespace PrintIt.Core.Tests.Pdfium
{
    public sealed class PdfPageTests
    {
        [Fact]
        public void Document_ShouldPointToDocumentContainingPage()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);
            using PdfPage page = document.OpenPage(0);

            // Act & Assert
            page.Document.Should().Be(document);
        }

        [Fact]
        public void SizeInInch_ShouldReturnCorrectSize()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);
            using PdfPage page = document.OpenPage(0);

            // Act
            SizeF size = page.SizeInInch;

            // Assert
            size.Width.Should().BeApproximately(8.5f, 0.01f);
            size.Height.Should().BeApproximately(11.0f, 0.01f);
        }

        [Fact]
        public void RenderTo_ShouldRenderPageToGraphics()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);
            using PdfPage page = document.OpenPage(0);
            using var bitmap = new Bitmap(1000, 1000, PixelFormat.Format24bppRgb);
            bitmap.SetResolution(300.0f, 300.0f);

            // Act
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                page.RenderTo(graphics, graphics.VisibleClipBounds);
            }

            // Assert
            // bitmap.Save("dummy-snapshot.png", ImageFormat.Png);
            CompareWithSnapshot(bitmap, "dummy-snapshot.png").Should().BeTrue();
        }

        private static bool CompareWithSnapshot(Bitmap bitmap, string snapshotImageName)
        {
            using var snapshot = (Bitmap)Image.FromStream(GetEmbeddedResourceStream(snapshotImageName));

            if (snapshot.Width != bitmap.Width || snapshot.Height != bitmap.Height)
                return false;

            for (var y = 0; y < snapshot.Height; y++)
            {
                for (var x = 0; x < snapshot.Width; x++)
                {
                    if (snapshot.GetPixel(x, y) != bitmap.GetPixel(x, y))
                        return false;
                }
            }

            return true;
        }

        private static Stream GetEmbeddedResourceStream(string name)
        {
            Type type = typeof(PdfPageTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}
