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
        private const string IsEmpty = "Get {request is null or empty!";
        private readonly ILogger<LinguisticCorpusFillmentService> Logger;
        private readonly IRepositoryAsync Repository;
        private readonly IHttpFeynmanTechniqueScraper HttpScraperContext;
        private readonly IHttpFeynmanTechniqueCore HttpCoreContext;
        private readonly IValidator<ScrapCriteria> ScrapValidator;

        public LinguisticCorpusFillmentService(
            ILogger<LinguisticCorpusFillmentService> logger,
            IRepositoryAsync repository,
            IHttpFeynmanTechniqueScraper httpScraperContext,
            IHttpFeynmanTechniqueCore httpCoreContext,
            IValidator<ScrapCriteria> scrapValidator)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            HttpScraperContext = httpScraperContext ?? throw new ArgumentNullException(nameof(httpScraperContext));
            HttpCoreContext = httpCoreContext ?? throw new ArgumentNullException(nameof(httpCoreContext));
            ScrapValidator = scrapValidator ?? throw new ArgumentNullException(nameof(scrapValidator));
        }

        public async Task<bool> ManualPostAsync(List<string> words, CancellationToken cancellationToken)
        {
            if ((words?.Count ?? 0) == 0)
            {
                Logger.LogError(IsEmpty, nameof(words));
                return false;
            }

            List<TokenResponse> tokens = await SpecifyPartOfSpeechAsync(words, cancellationToken);
            if ((tokens?.Count ?? 0) == 0)
            {
                Logger.LogError(IsEmpty, nameof(tokens));
                return false;
            }

            return await BulkInsertWordsTransaction(PrepareWords(tokens), cancellationToken);
        }

        public async Task<bool> PostAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            ValidationResult validatorResult = await ScrapValidator.ValidateAsync(criteria, cancellationToken);
            if (!validatorResult.IsValid)
            {
                Logger.LogError(ErrorMessage, nameof(ScrapServiceDto), nameof(ScrapCriteria));
                return false;
            }

            List<WordDto> wordDtos = await ScrapFromResourceAsync(criteria, cancellationToken);
            if ((wordDtos?.Count ?? 0) == 0)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(ScrapCriteria));
                return false;
            }

            List<ScraperTokenResponse> scraperTokens = await SpecifyPartOfSpeechAsync(wordDtos.Distinct().ToList(), cancellationToken);
            if ((scraperTokens?.Count ?? 0) == 0)
            {
                Logger.LogError(ErrorMessage, nameof(ScraperTokenResponse), nameof(WordDto));
                return false;
            }

            return await BulkInsertWordsTransaction(PrepareWords(scraperTokens), cancellationToken);
        }

        private async Task<List<ScraperTokenResponse>> SpecifyPartOfSpeechAsync(List<WordDto> wordDtos, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpCoreContext.PrepareAddress(FeynmanTechniqueCoreUrl.AnalyzeSpeeches);
            RestRequest? restRequest = HttpCoreContext.PrepareRequest(uri, Method.Post, wordDtos);
            if (restRequest == null)
            {
                Logger.LogError(ErrorMessage, nameof(Word), nameof(RestRequest));
                return new List<ScraperTokenResponse>();
            }

            return await client.PostAsync<List<ScraperTokenResponse>>(restRequest, cancellationToken) ?? new();
        }

        private async Task<List<TokenResponse>> SpecifyPartOfSpeechAsync(List<string> words, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpCoreContext.PrepareAddress(FeynmanTechniqueCoreUrl.AnalyzeSpeechesText);
            RestRequest? restRequest = HttpCoreContext.PrepareRequest(uri, Method.Post, words);
            if (restRequest == null)
            {
                Logger.LogError(ErrorMessage, nameof(TokenResponse), nameof(RestRequest));
                return new List<TokenResponse>();
            }

            return await client.PostAsync<List<TokenResponse>>(restRequest, cancellationToken) ?? new();
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

        private async Task<bool> BulkInsertWordsTransaction(List<Word> preparedWords, CancellationToken cancellationToken)
        {
            IDbContextTransaction dbContextTransaction = await Repository.BeginTransactionAsync(cancellationToken);
            bool succeeded = false;
            try
            {
                Logger.LogInformation(preparedWords.LogCollection());
                await Repository.BulkInsertAsync(preparedWords.AsEnumerable(), cancellationToken);
                succeeded = true;
                return succeeded;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex.GetFullMessage());
                return succeeded;
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

        private static List<Word> PrepareWords(List<ScraperTokenResponse> detailedWords)
        {
            List<Word> words = new();
            DateTime now = DateTime.UtcNow;
            foreach (ScraperTokenResponse detailedWord in detailedWords)
            {
                words.AddRange(detailedWord.Words.Select(s => new Word
                {
                    Name = s.Lemma,
                    PartOfSpeech = !string.IsNullOrEmpty(s.PartOfSpeech) ? s.PartOfSpeech.MapPartOfSpeech() : 0,
                    CreatedDate = now,
                    Context = "scraper",
                    Link = detailedWord.Source
                }));
            }
            return words;
        }

        private static List<Word> PrepareWords(List<TokenResponse> tokens)
        {
            DateTime now = DateTime.UtcNow;
            return tokens.Select(s => new Word
            {
                Name = s.Lemma,
                PartOfSpeech = !string.IsNullOrEmpty(s.PartOfSpeech) ? s.PartOfSpeech.MapPartOfSpeech() : 0,
                CreatedDate = now,
                Context = "dashboard",
                Link = string.Empty
            }).ToList();
        }
    }
}
