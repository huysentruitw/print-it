using System.IO;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.TokenParsers
{
    internal abstract class RangeTokenParser
    {
        public static Token Parse(TextReader reader)
        {
            if (IsRangeToken(reader))
            {
                reader.Read();
                return new RangeToken();
            }

            return null;
        }

        private static bool IsRangeToken(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == '-';
        }
    }
}
