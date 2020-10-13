using System.IO;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.TokenParsers
{
    internal static class SeparatorTokenParser
    {
        public static Token Parse(TextReader reader)
        {
            if (IsSeparatorToken(reader))
            {
                var separator = (char)reader.Read();
                return new SeparatorToken(separator);
            }

            return null;
        }

        private static bool IsSeparatorToken(TextReader reader)
        {
            var peek = (char)reader.Peek();
            return peek == ',' || peek == ';';
        }
    }
}
