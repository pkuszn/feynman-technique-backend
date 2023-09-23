namespace FeynmanTechniqueBackend.Extensions
{
    public static class LoggerExtension
    {
        private const string Null = "<null>";
        public static string LogCollection<T>(this List<T> collection)
            where T : class
        {
            if (collection == null)
            {
                return Null;
            }

            return string.Join("\n", collection.Select(s => s.ToString()).ToList());
        }
    }
}
