namespace FeynmanTechniqueBackend.Extensions
{
    public static class ExceptionExtensions
    {
        private const string Format = "{exception}: {innerException}";
        private const string Null = "<null>";

        public static string GetFullMessage(this Exception exception) => string.Format(Format, exception.Message, exception.InnerException?.Message ?? Null);
    }
}
