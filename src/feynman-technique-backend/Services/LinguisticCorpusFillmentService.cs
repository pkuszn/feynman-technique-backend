using FeynmanTechniqueBackend.Controllers.Criteria;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.HttpModels.Models;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using FeynmanTechniqueBackend.Services.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore.Storage;
using RestSharp;
using static FeynmanTechniqueBackend.Constants.Addresses;

namespace FeynmanTechniqueBackend.Services
{
    public class LinguisticCorpusFillmentService : ILinguisticCorpusFillmentService
    {
        private const string ErrorMessage = "Get {response} failed. {request} is null or empty.";
        private readonly ILogger<LinguisticCorpusFillmentService> Logger;
        private readonly IRepositoryAsync Repository;
        private readonly IHttpFeynmanTechniqueScraper HttpScraperContext;
        private readonly IHttpFeynmanTechniqueCore HttpCoreContext;
        private readonly IValidator<ScrapCriteria> Validator;

        public LinguisticCorpusFillmentService(
            ILogger<LinguisticCorpusFillmentService> logger,
            IRepositoryAsync repository,
            IHttpFeynmanTechniqueScraper httpScraperContext,
            IHttpFeynmanTechniqueCore httpCoreContext,
            IValidator<ScrapCriteria> validator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            HttpScraperContext = httpScraperContext ?? throw new ArgumentNullException(nameof(httpScraperContext));
            HttpCoreContext = httpCoreContext ?? throw new ArgumentNullException(nameof(httpCoreContext));
            Validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<List<Word>> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            ValidationResult validatorResult = await Validator.ValidateAsync(criteria, cancellationToken);
            if (!validatorResult.IsValid)
            {
                Logger.LogError(ErrorMessage, nameof(ScrapServiceDto), nameof(ScrapCriteria));
                return new();
            }

            List<WordDto> wordDtos = await ScrapFromResourceAsync(criteria, cancellationToken);
            if ((wordDtos?.Count ?? 0) == 0)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(ScrapCriteria));
                return new();
            }

            List<DetailedWordResponse> detailedWords = await SpecifyPartOfSpeechAsync(wordDtos.Distinct().ToList(), cancellationToken);
            if ((detailedWords?.Count ?? 0) == 0)
            {
                Logger.LogError(ErrorMessage, nameof(DetailedWordResponse), nameof(WordDto));
                return new();
            }

            List<Word> preparedWords = PrepareWords(detailedWords);
            if ((preparedWords?.Count ?? 0) == 0)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(DetailedWordResponse));
                return new();
            }

            await BulkInsertWordsTransaction(preparedWords, cancellationToken);

            return preparedWords;
        }

        private async Task<List<DetailedWordResponse>> SpecifyPartOfSpeechAsync(List<WordDto> wordDtos, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpCoreContext.PrepareAddress(FeynmanTechniqueCoreUrl.AnalyzeSpeeches);
            RestRequest? restRequest = HttpCoreContext.PrepareRequest(uri, Method.Post, wordDtos);
            if (restRequest == null)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(RestRequest));
                return new List<DetailedWordResponse>();
            }

            return await client.PostAsync<List<DetailedWordResponse>>(restRequest, cancellationToken) ?? new();
        }

        private async Task<List<WordDto>> ScrapFromResourceAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpScraperContext.PrepareAddress(FeynmanTechniqueScraperUrl.Many);
            RestRequest? restRequest = HttpScraperContext.PrepareRequest(uri, Method.Post, criteria);
            if (restRequest == null)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(RestRequest));
                return new List<WordDto>();
            }

            return await client.PostAsync<List<WordDto>>(restRequest, cancellationToken) ?? new();
        }

        private async Task BulkInsertWordsTransaction(List<Word> preparedWords, CancellationToken cancellationToken)
        {
            IDbContextTransaction dbContextTransaction = await Repository.BeginTransactionAsync(cancellationToken);
            bool succeeded = false;
            try
            {
                Logger.LogInformation(string.Join("\n", preparedWords.ToString()));
                await Repository.BulkInsertAsync(preparedWords.AsEnumerable(), cancellationToken);
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

        private static List<Word> PrepareWords(List<DetailedWordResponse> detailedWords)
        {
            List<Word> words = new();
            DateTime now = DateTime.UtcNow;
            foreach (DetailedWordResponse detailedWord in detailedWords)
            {
                words.AddRange(detailedWord.Words.Select(s => new Word
                {
                    Name = s.Lemma ?? string.Empty,
                    PartOfSpeech = !string.IsNullOrEmpty(s.PartOfSpeech) ? s.PartOfSpeech.MapPartOfSpeech() : 0,
                    CreatedDate = now,
                    Context = "scraper",
                    Link = detailedWord.Source
                }));
            }
            return words;
        }
    }
}
