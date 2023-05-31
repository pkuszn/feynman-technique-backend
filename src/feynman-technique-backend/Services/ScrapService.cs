using FeynmanTechniqueBackend.Constants;
using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage;
using RestSharp;
using static FeynmanTechniqueBackend.Constants.Addresses;

namespace FeynmanTechniqueBackend.Services
{
    public class ScrapService : IScrapService
    {
        private readonly ILogger<ScrapService> Logger;
        private readonly FeynmanTechniqueBackendContext DbContext;
        private readonly IHttpFeynmanTechniqueScraper HttpScraperContext;
        private readonly IValidator<ScrapCriteria> Validator;

        public ScrapService(
            ILogger<ScrapService> logger,
            FeynmanTechniqueBackendContext dbContext,
            IHttpFeynmanTechniqueScraper httpScraperContext,
            IValidator<ScrapCriteria> validator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            HttpScraperContext = httpScraperContext ?? throw new ArgumentNullException(nameof(httpScraperContext));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<ScrapDto> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            FluentValidation.Results.ValidationResult validatorResult = await Validator.ValidateAsync(criteria, cancellationToken);
            if (!validatorResult.IsValid)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(ScrapDto), nameof(ScrapCriteria));
                return new ScrapDto();
            }

            RestClient client = new();
            Uri uri = HttpScraperContext.PrepareAddress(FeynmanTechniqueScraperUrl.Many);
            RestRequest? restRequest = HttpScraperContext.PrepareRequest(uri, Method.Post, criteria.Links);
            if (restRequest == null)
            {
                Logger.LogError("Creating {request} failed. {request} is null or empty.", nameof(RestRequest));
                return new ScrapDto();
            }

            RestResponse response = await client.GetAsync(restRequest, cancellationToken);
            return new();
        }

        private async Task<bool> BulkInsertWordsTransaction(HashSet<string> words)
        {
            IDbContextTransaction dbContextTransaction = DbContext.Database.BeginTransaction();
            bool succeeded = false;
            try
            {

            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.GetFullMessage());
            }
            finally
            {
                if (succeeded)
                {
                    dbContextTransaction.Commit();
                }
                else
                {
                    dbContextTransaction.Rollback();
                }
            }
        }
    }
}
