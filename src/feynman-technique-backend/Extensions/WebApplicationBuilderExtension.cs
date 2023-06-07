using FeynmanTechniqueBackend.HttpModels;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository;
using FeynmanTechniqueBackend.Repository.Interfaces;
using FeynmanTechniqueBackend.Services;
using FeynmanTechniqueBackend.Services.Interfaces;
using FeynmanTechniqueBackend.Validators;
using FluentValidation;

namespace FeynmanTechniqueBackend.Extensions
{
    internal static class WebApplicationBuilderExtension
    {
        public static WebApplicationBuilder AddDatabases(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddDbContext<FeynmanTechniqueBackendContext>()
                .AddScoped<IRepositoryAsync, RepositoryAsync>();

            return builder;
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IServiceUtilitiesService, ServiceUtilitiesService>()
                .AddScoped<ILinguisticCorpusFillmentService, LinguisticCorpusFillmentService>()
                .AddScoped<IHttpFeynmanTechniqueScraper, HttpFeynmanTechniqueScraper>()
                .AddScoped<IHttpFeynmanTechniqueCore, HttpFeynmanTechniqueCore>();

            return builder;
        }

        public static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IValidator<ScrapCriteria>, ScrapValidator>();

            return builder;
        }
    }
}
