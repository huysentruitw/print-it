using System.Collections.Generic;
using System.IO;
using System.Linq;
using PrintIt.Core.Internal.PageRangeString;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal
{
    internal static class PageRangeStringParser
    {
        public static int[] Parse(string pageRange, int totalNumberOfPages)
        {
            if (string.IsNullOrWhiteSpace(pageRange))
                return Enumerable.Range(1, totalNumberOfPages).ToArray();

            using var reader = new StringReader(pageRange);
            IEnumerable<Token> tokens = Tokenizer.Tokenize(reader);
            IEnumerable<object> sequence = Reducer.Reduce(tokens, totalNumberOfPages);
            return Emitter.Emit(sequence, totalNumberOfPages).ToArray();
        }
    }
}
