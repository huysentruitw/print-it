using System.IO;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.TokenParsers
{
    internal static class PageNumberTokenParser
    {
        public static Token Parse(TextReader reader)
        {
            if (IsPageNumber(reader))
            {
                int pageNumber = AccumulatePageNumber(reader);
                return new PageNumberToken(pageNumber);
            }

            return null;
        }

        private static bool IsPageNumber(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return IsDigit(peek);
        }

        private static bool IsDigit(char value)
            => value >= '0' && value <= '9';

        private static int AccumulatePageNumber(TextReader reader)
        {
            int result = reader.Read() - '0';

            while (IsDigit((char)reader.Peek()))
            {
                result *= 10;
                result += reader.Read() - '0';
            }

            return result;
        }
    }
}
