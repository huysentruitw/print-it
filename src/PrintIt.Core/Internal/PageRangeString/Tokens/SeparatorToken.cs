namespace PrintIt.Core.Internal.PageRangeString.Tokens
{
    internal sealed class SeparatorToken : Token
    {
        private readonly char _separator;

        public SeparatorToken(char separator)
        {
            _separator = separator;
        }

        public override string ToString() => _separator.ToString();
    }
}
