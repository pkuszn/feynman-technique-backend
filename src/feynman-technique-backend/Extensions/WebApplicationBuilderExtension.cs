using FeynmanTechniqueBackend.Configuration;
using FeynmanTechniqueBackend.Controllers.Criteria;
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

            builder.Services.AddDbContext<FeynmanTechniqueCorpusContext>()
                .AddScoped<IRepositoryAsync, RepositoryAsync>();

            return builder;
        }

        public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IServiceUtilitiesService, ServiceUtilitiesService>()
                .AddScoped<ILinguisticCorpusFillmentService, LinguisticCorpusFillmentService>()
                .AddScoped<IHttpFeynmanTechniqueScraper, HttpFeynmanTechniqueScraper>()
                .AddScoped<IHttpFeynmanTechniqueCore, HttpFeynmanTechniqueCore>()
                .AddScoped<IUserManagementService, UserManagementService>();

            return builder;
        }

        public static WebApplicationBuilder AddValidators(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<IValidator<ScrapCriteria>, ScrapValidator>()
                .AddScoped<IValidator<ValidateUserCriteria>, ValidateUserValidator>();

            return builder;
        }

        public static WebApplicationBuilder AddConfiguration(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.Configure<FeynmanTechniqueScraperOptions>(
                builder.Configuration.GetSection(FeynmanTechniqueScraperOptions.FeynmanTechniqueScraperConfiguration));

            builder.Services.Configure<FeynmanTechniqueCoreOptions>(
                builder.Configuration.GetSection(FeynmanTechniqueCoreOptions.FeynmanTechniqueCoreConfiguration));

            return builder;
        }

        public static WebApplicationBuilder AddCors(this WebApplicationBuilder builder)
        {
            _ = builder ?? throw new ArgumentNullException(nameof(builder));

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin() // You can restrict this to specific origins by using WithOrigins("http://localhost:3000")
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            return builder;
        }
    }
}
