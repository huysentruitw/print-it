namespace PrintIt.Core.Internal.PageRangeString.Tokens
{
    internal sealed class PageNumberToken : Token
    {
        public PageNumberToken(int pageNumber)
        {
            PageNumber = pageNumber;
        }

        public int PageNumber { get; }

        public override string ToString() => PageNumber.ToString();
    }
}
