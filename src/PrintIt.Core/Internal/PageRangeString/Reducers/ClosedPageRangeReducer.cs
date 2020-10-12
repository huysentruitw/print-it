using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.Reducers
{
    internal static class ClosedPageRangeReducer
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

                    var rangeToken = enumerator.Current as RangeToken;

                    if (!(GetNext(enumerator) is PageNumberToken))
                    {
                        yield return fromPageNumberToken;
                        yield return rangeToken;
                        yield return enumerator.Current;
                        continue;
                    }

                    var toPageNumberToken = enumerator.Current as PageNumberToken;

                    Validate(fromPageNumberToken.PageNumber, toPageNumberToken.PageNumber, totalNumberOfPages);
                    yield return new ClosedPageRangeNode(fromPageNumberToken.PageNumber, toPageNumberToken.PageNumber);

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

        private static void Validate(int from, int to, int totalNumberOfPages)
        {
            if (from < 1) throw new PageRangeStringOutOfRangeException($"Range 'from' value out of range in '{from}-{to}'");
            if (from > totalNumberOfPages) throw new PageRangeStringOutOfRangeException($"Range 'from' value cannot be greater than total number of pages ({totalNumberOfPages}) in '{from}-{to}'");
            if (to < from) throw new PageRangeStringOutOfRangeException($"Range 'to' value cannot be less than range 'from' value in '{from}-{to}'");
            if (to > totalNumberOfPages) throw new PageRangeStringOutOfRangeException($"Range 'to' value cannot be greater than total number of pages ({totalNumberOfPages}) in '{from}-{to}'");
        }
    }

    internal sealed class ClosedPageRangeNode
    {
        public ClosedPageRangeNode(int from, int to)
        {
            From = from;
            To = to;
        }

        public int From { get; }

        public int To { get; }
    }
}
