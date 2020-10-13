using System;
using System.Linq;
using PrintIt.Core.Internal.PageRangeString;
using PrintIt.Core.Pdfium;

namespace PrintIt.Core.Internal
{
    internal static class PrintStateFactory
    {
        public static PrintState Create(PdfDocument document, string pageRange = null)
            => Create(document, document.PageCount, pageRange);

        internal static PrintState Create(PdfDocument document, int documentPageCount, string pageRange = null)
        {
            if (documentPageCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(documentPageCount));

            int[] pageNumbers = PageRangeStringParser.Parse(pageRange, totalNumberOfPages: documentPageCount);
            if (!pageNumbers.Any())
                throw new PageRangeStringFormatException($"Failed to parse page range string: {pageRange}");

            return new PrintState(document, pageNumbers);
        }
    }

    internal sealed class PrintState
    {
        private readonly int[] _pageNumbers;
        private int _cursor;

        public PrintState(PdfDocument document, int[] pageNumbers)
        {
            _pageNumbers = pageNumbers;
            Document = document;
            _cursor = 0;
        }

        public PdfDocument Document { get; }

        public int CurrentPageIndex => _pageNumbers[_cursor] - 1;

        public bool AdvanceToNextPage()
        {
            if (_cursor >= _pageNumbers.Length)
                return false;

            _cursor++;
            return _cursor < _pageNumbers.Length;
        }
    }
}
