using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Services;
using FeynmanTechniqueBackend.Services.Interfaces;

namespace FeynmanTechniqueBackend.Extensions
{
    internal static class WebApplicationBuilderExtension
    {
        public static WebApplicationBuilder AddDatabases(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            
             builder.Services.AddDbContext<FeynmanTechniqueBackendContext>();

             return builder;
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));
            
            builder.Services.AddScoped<IServiceUtilitiesService, ServiceUtilitiesService>();

            return builder;
        }
    }
}
