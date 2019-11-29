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
            using var stream = GetEmbeddedResourceStream("dummy.pdf");
            using var document = PdfDocument.Open(stream);
            using var page = document.OpenPage(0);
            
            // Act & Assert
            page.Document.Should().Be(document);
        }

        [Fact]
        public void SizeInInch_ShouldReturnCorrectSize()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using var stream = GetEmbeddedResourceStream("dummy.pdf");
            using var document = PdfDocument.Open(stream);
            using var page = document.OpenPage(0);
            
            // Act
            var size = page.SizeInInch;
            
            // Assert
            size.Width.Should().BeApproximately(8.26f, 0.01f);
            size.Height.Should().BeApproximately(11.69f, 0.01f);
        }

        [Fact]
        public void RenderTo_ShouldRenderPageToGraphics()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using var stream = GetEmbeddedResourceStream("dummy.pdf");
            using var document = PdfDocument.Open(stream);
            using var page = document.OpenPage(0);
            using var bitmap = new Bitmap(2000, 1000, PixelFormat.Format24bppRgb);

            // Act
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.White);
                page.RenderTo(graphics);
            }

            // Assert
            bitmap.Save("dummy-output.png", ImageFormat.Png);
        }
        
        private static Stream GetEmbeddedResourceStream(string name)
        {
            var type = typeof(PdfDocumentTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}