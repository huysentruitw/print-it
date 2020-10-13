using System;

namespace PrintIt.Core.Internal.PageRangeString
{
    internal abstract class PageRangeStringException : Exception
    {
        protected PageRangeStringException(string message)
            : base(message)
        {
        }
    }

    internal sealed class PageRangeStringFormatException : PageRangeStringException
    {
        public PageRangeStringFormatException(string message)
            : base(message)
        {
        }
    }

    internal sealed class PageRangeStringOutOfRangeException : PageRangeStringException
    {
        public PageRangeStringOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}
