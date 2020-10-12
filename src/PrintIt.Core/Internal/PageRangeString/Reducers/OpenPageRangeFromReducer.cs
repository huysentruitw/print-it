using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.Reducers
{
    internal static class OpenPageRangeFromReducer
    {
        public static IEnumerable<object> Reduce(IEnumerable<object> sequence, int totalNumberOfPages)
        {
            IEnumerator<object> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is PageNumberToken)
                {
                    var fromPageNumberToken = enumerator.Current as PageNumberToken;

                    if (!(GetNext(enumerator) is RangeToken))
                    {
                        yield return fromPageNumberToken;
                        yield return enumerator.Current;
                        continue;
                    }

                    Validate(fromPageNumberToken.PageNumber, totalNumberOfPages);
                    yield return new OpenPageRangeFromNode(fromPageNumberToken.PageNumber);

                    // The next token should be a separator or end of sequence
                    var nextToken = GetNext(enumerator);
                    if (!(nextToken is SeparatorToken || nextToken is EndToken))
                    {
                        throw new PageRangeStringFormatException($"Expected separator or end token instead of {nextToken}");
                    }
                }
                else
                {
                    yield return enumerator.Current;
                }
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
            => enumerator.MoveNext() ? enumerator.Current : null;

        private static void Validate(int from, int totalNumberOfPages)
        {
            if (from < 1) throw new PageRangeStringOutOfRangeException($"Range 'from' value out of range in '{from}-'");
            if (from > totalNumberOfPages) throw new PageRangeStringOutOfRangeException($"Range 'from' value cannot be greater than total number of pages ({totalNumberOfPages}) in '{from}-'");
        }
    }

    internal sealed class OpenPageRangeFromNode
    {
        public OpenPageRangeFromNode(int from)
        {
            From = from;
        }

        public int From { get; }
    }
}
