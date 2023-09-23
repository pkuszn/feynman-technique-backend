using Newtonsoft.Json.Serialization;

namespace FeynmanTechniqueBackend.Extensions
{
    public static class StringExtension
    {
        //TODO: ToPascalStrategy
        public static string? ToSnakeCase(this string? str) => str is null
            ? null
            : new DefaultContractResolver() { NamingStrategy = new SnakeCaseNamingStrategy() }.GetResolvedPropertyName(str);
    }
}
