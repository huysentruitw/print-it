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
        
        private static Stream GetEmbeddedResourceStream(string name)
        {
            var type = typeof(PdfDocumentTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}