using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Tokens;

namespace PrintIt.Core.Internal.PageRangeString.Reducers
{
    internal static class SinglePageReducer
    {
        public static IEnumerable<object> Reduce(IEnumerable<object> sequence, int totalNumberOfPages)
        {
            IEnumerator<object> enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is PageNumberToken)
                {
                    var pageNumber = enumerator.Current as PageNumberToken;
                    Validate(pageNumber.PageNumber, totalNumberOfPages);
                    yield return new SinglePageNode(pageNumber.PageNumber);

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

        private static void Validate(int pageNumber, int totalNumberOfPages)
        {
            if (pageNumber < 1) throw new PageRangeStringOutOfRangeException($"Page number {pageNumber} is out of range");
            if (pageNumber > totalNumberOfPages) throw new PageRangeStringOutOfRangeException($"Page number cannot be greater than total number of pages ({totalNumberOfPages}) but was {pageNumber}");
        }
    }

    internal sealed class SinglePageNode
    {
        public SinglePageNode(int number)
        {
            Number = number;
        }

        public int Number { get; }
    }
}
