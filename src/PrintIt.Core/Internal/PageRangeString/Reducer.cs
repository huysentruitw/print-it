using System.Collections.Generic;
using PrintIt.Core.Internal.PageRangeString.Reducers;

namespace PrintIt.Core.Internal.PageRangeString
{
    internal static class Reducer
    {
        public static IEnumerable<object> Reduce(IEnumerable<object> tokens, int totalNumberOfPages)
        {
            tokens = ClosedPageRangeReducer.Reduce(tokens, totalNumberOfPages);
            tokens = OpenPageRangeFromReducer.Reduce(tokens, totalNumberOfPages);
            tokens = OpenPageRangeToReducer.Reduce(tokens, totalNumberOfPages);
            tokens = SinglePageReducer.Reduce(tokens, totalNumberOfPages);
            return tokens;
        }
    }
}
