using System;
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
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");

            // Act
            using PdfDocument document = PdfDocument.Open(stream);

            // Assert
            document.Should().NotBeNull();
        }

        [Fact]
        public void PageCount_ReturnsPageCountOfDocument()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);

            // Act
            var pageCount = document.PageCount;

            // Assert
            pageCount.Should().Be(1);
        }

        [Fact]
        public void OpenPage_ReturnsRequestedPage()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);

            // Act
            using PdfPage page = document.OpenPage(0);

            // Assert
            page.Should().NotBeNull();
        }

        [Fact]
        public void OpenPage_IndexOutOfRange_ShouldThrowIndexOutOfRangeException()
        {
            // Arrange
            PdfLibrary.EnsureInitialized();
            using Stream stream = GetEmbeddedResourceStream("dummy.pdf");
            using PdfDocument document = PdfDocument.Open(stream);

            // Act
            Action action = () => document.OpenPage(5);

            // Assert
            action.Should().Throw<IndexOutOfRangeException>();
        }

        private static Stream GetEmbeddedResourceStream(string name)
        {
            Type type = typeof(PdfDocumentTests);
            return type.Assembly.GetManifestResourceStream(type, name);
        }
    }
}
