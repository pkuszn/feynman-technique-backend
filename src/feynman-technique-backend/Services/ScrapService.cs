using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.HttpModels.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
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
        private readonly IRepositoryAsync Repository;
        private readonly IHttpFeynmanTechniqueScraper HttpScraperContext;
        private readonly IValidator<ScrapCriteria> Validator;

        public ScrapService(
            ILogger<ScrapService> logger,
            IRepositoryAsync repository,
            IHttpFeynmanTechniqueScraper httpScraperContext,
            IValidator<ScrapCriteria> validator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
            RestRequest? restRequest = HttpScraperContext.PrepareRequest(uri, Method.Post, criteria);
            if (restRequest == null)
            {
                Logger.LogError("Creating {request} failed. {request} is null or empty.", nameof(RestRequest));
                return new ScrapDto();
            }

            List<Words>? response = await client.PostAsync<List<Words>>(restRequest, cancellationToken);
            if ((response?.Count ?? 0) == 0)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(Words), nameof(RestRequest));
                return new ScrapDto();
            }

            HashSet<string> words = response.SelectMany(m => m.WordList.Select(s => s).Distinct()).ToHashSet();
            //TODO: Określenie części mowy teraz. Nowy serwis potrzebny
            await BulkInsertWordsTransaction(words, cancellationToken);

            return new ScrapDto()
            {
                Words = words
            };
        }

        private async Task BulkInsertWordsTransaction(HashSet<string> words, CancellationToken cancellationToken)
        {
            IDbContextTransaction dbContextTransaction = await Repository.BeginTransactionAsync(cancellationToken);
            bool succeeded = false;
            try
            {
                await Repository.BulkInsertAsync(words.AsEnumerable(), cancellationToken);
                succeeded = true;
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
