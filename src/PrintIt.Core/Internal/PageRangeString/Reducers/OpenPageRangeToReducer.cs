using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.Reducers
{
    internal static class OpenPageRangeToReducer
    {
        public static IEnumerable<object> Reduce(IEnumerable<object> sequence, int totalNumberOfPages)
        {
            IEnumerator<object> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is RangeToken)
                {
                    var rangeToken = enumerator.Current as RangeToken;

                    if (!(GetNext(enumerator) is PageNumberToken))
                    {
                        yield return rangeToken;
                        yield return enumerator.Current;
                        continue;
                    }

                    var toPageNumberToken = enumerator.Current as PageNumberToken;
                    Validate(toPageNumberToken.PageNumber, totalNumberOfPages);
                    yield return new OpenPageRangeToNode(toPageNumberToken.PageNumber);

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

        private static void Validate(int to, int totalNumberOfPages)
        {
            if (to < 1) throw new PageRangeStringOutOfRangeException($"Range 'to' value out of range in '-{to}'");
            if (to > totalNumberOfPages) throw new PageRangeStringOutOfRangeException($"Range 'to' value cannot be greater than total number of pages ({totalNumberOfPages}) in '-{to}'");
        }
    }

    internal sealed class OpenPageRangeToNode
    {
        public OpenPageRangeToNode(int to)
        {
            To = to;
        }

        public int To { get; }
    }
}
