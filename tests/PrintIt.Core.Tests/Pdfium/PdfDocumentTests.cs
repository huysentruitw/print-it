using System.IO;
using FluentAssertions;
using PrintIt.Core.Pdfium;
using Xunit;

namespace PrintIt.Core.Tests.Pdfium
{
    public sealed class PdfDocumentTests
    {
        [Fact]
        public void Open_CanOpenDocumentFromStream()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using var stream = GetEmbeddedResourceStream("dummy.pdf");
            
            // Act
            using var document = PdfDocument.Open(stream);
            
            // Assert
            document.Should().NotBeNull();
        }

        [Fact]
        public void PageCount_ReturnsPageCountOfDocument()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using var stream = GetEmbeddedResourceStream("dummy.pdf");
            using var document = PdfDocument.Open(stream);
            
            // Act
            var pageCount = document.PageCount;
            
            // Assert
            pageCount.Should().Be(1);
        }
        
        private static Stream GetEmbeddedResourceStream(string name)
        {
            var type = typeof(PdfDocumentTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}
