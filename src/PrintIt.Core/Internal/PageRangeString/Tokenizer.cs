using System.Collections.Generic;
using System.IO;
using PrintIt.Core.Internal.PageRangeString.TokenParsers;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString
{
    internal static class Tokenizer
    {
        public static IEnumerable<Token> Tokenize(TextReader source)
        {
            while (source.Peek() != -1)
            {
                Token token = null;
                token ??= PageNumberTokenParser.Parse(source);
                token ??= RangeTokenParser.Parse(source);
                token ??= SeparatorTokenParser.Parse(source);

                if (token == null)
                {
                    // Tokens can only be separated with whitespace
                    char unparsableChar = (char)source.Read();
                    if (!char.IsWhiteSpace(unparsableChar))
                        throw new PageRangeStringFormatException($"Invalid character found in source: {unparsableChar}");
                    continue;
                }

                yield return token;
            }

            yield return new EndToken();
        }
    }
}
