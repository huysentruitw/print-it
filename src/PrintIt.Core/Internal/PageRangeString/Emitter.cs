using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Reducers;

namespace PrintIt.Core.Internal.PageRangeString
{
    internal static class Emitter
    {
        public static IEnumerable<int> Emit(IEnumerable<object> sequence, int totalNumberOfPages)
        {
            foreach (var node in sequence)
            {
                switch (node)
                {
                case SinglePageNode singlePage:
                    yield return singlePage.Number;
                    break;
                case ClosedPageRangeNode closedPageRange:
                    for (int pageNumber = closedPageRange.From; pageNumber <= closedPageRange.To; pageNumber++)
                        yield return pageNumber;
                    break;
                case OpenPageRangeFromNode openPageRangeFrom:
                    for (int pageNumber = openPageRangeFrom.From; pageNumber <= totalNumberOfPages; pageNumber++)
                        yield return pageNumber;
                    break;
                case OpenPageRangeToNode openPageRangeTo:
                    for (int pageNumber = 1; pageNumber <= openPageRangeTo.To; pageNumber++)
                        yield return pageNumber;
                    break;
                }
            }
        }
    }
}
