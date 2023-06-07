using FeynmanTechniqueBackend.DtoModels;
using FeynmanTechniqueBackend.Extensions;
using FeynmanTechniqueBackend.HttpModels.Interfaces;
using FeynmanTechniqueBackend.HttpModels.Models;
using FeynmanTechniqueBackend.Models;
using FeynmanTechniqueBackend.Repository.Interfaces;
using FeynmanTechniqueBackend.Services.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Storage;
using RestSharp;
using static FeynmanTechniqueBackend.Constants.Addresses;

namespace FeynmanTechniqueBackend.Services
{
    public class LinguisticCorpusFillmentService : ILinguisticCorpusFillmentService
    {
        private readonly ILogger<LinguisticCorpusFillmentService> Logger;
        private readonly IRepositoryAsync Repository;
        private readonly IHttpFeynmanTechniqueScraper HttpScraperContext;
        private readonly IHttpFeynmanTechniqueCore HttpCoreContext;
        private readonly IValidator<ScrapCriteria> Validator;
        private Dictionary<int, string> IdToLinkDict { get; set; } = new();

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
            FluentValidation.Results.ValidationResult validatorResult = await Validator.ValidateAsync(criteria, cancellationToken);
            if (!validatorResult.IsValid)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(ScrapDto), nameof(ScrapCriteria));
                return new();
            }

            List<Words> words = await ScrapFromResourceAsync(criteria, cancellationToken);
            if ((words?.Count ?? 0) == 0)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(Words), nameof(ScrapCriteria));
                return new();
            }

            string[] links = words.Select(s => s.Source).Distinct().ToArray();
            if (links.Length == 0)
            {
                Logger.LogError("Create links array failed. {source} is null or empty.", nameof(Words.Source));
                return new();
            }

            MapLinkToId(links);

            List<InternalWord> internalWords = PrepareCorePayload(words);
            if (internalWords.Count == 0)
            {
                Logger.LogError("Get {entity} failed. {criteria} is null or empty.", nameof(InternalWord), nameof(Words));
                return new();
            }

            List<Word> preparedWords = PrepareWords(await SpecifyPartOfSpeechAsync(internalWords, cancellationToken) ?? new());
            await BulkInsertWordsTransaction(preparedWords, cancellationToken);

            return preparedWords;
        }

        private async Task<List<DetailedWord>> SpecifyPartOfSpeechAsync(List<InternalWord> internalWords, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpCoreContext.PrepareAddress(FeynmanTechniqueCoreUrl.AnalyzeSpeeches);
            RestRequest? restRequest = HttpCoreContext.PrepareRequest(uri, Method.Post, internalWords);
            if (restRequest == null)
            {
                Logger.LogError("Get {request} failed. {request} is null or empty.", nameof(Words), nameof(RestRequest));
                return new List<DetailedWord>();
            }

            return await client.PostAsync<List<DetailedWord>>(restRequest, cancellationToken) ?? new();
        }

        private async Task<List<Words>> ScrapFromResourceAsync(ScrapCriteria criteria, CancellationToken cancellationToken)
        {
            RestClient client = new();
            Uri uri = HttpScraperContext.PrepareAddress(FeynmanTechniqueScraperUrl.Many);
            RestRequest? restRequest = HttpScraperContext.PrepareRequest(uri, Method.Post, criteria);
            if (restRequest == null)
            {
                Logger.LogError("Get {request} failed. {request} is null or empty.", nameof(Words), nameof(RestRequest));
                return new List<Words>();
            }

            return await client.PostAsync<List<Words>>(restRequest, cancellationToken) ?? new();
        }

        private async Task BulkInsertWordsTransaction(List<Word> preparedWords, CancellationToken cancellationToken)
        {
            IDbContextTransaction dbContextTransaction = await Repository.BeginTransactionAsync(cancellationToken);
            bool succeeded = false;
            try
            {   
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

        private static List<Word> PrepareWords(List<DetailedWord> detailedWords)
        {
            if ((detailedWords?.Count ?? 0) == 0)
            {
                return new();
            }

            DateTime dateTimeNow = DateTime.UtcNow;
            return detailedWords.Select(s => new Word()
            {
                Id = 0,
                Name = s.Lemma ?? string.Empty,
                PartOfSpeech = !string.IsNullOrEmpty(s.PartOfSpeech) ? s.PartOfSpeech.MapPartOfSpeech() : 0,
                CreatedDate = dateTimeNow,
                Context = "scraper",
                Link = "null"
            }).ToList();
        }

        private List<InternalWord> PrepareCorePayload(List<Words> words)
        {
            List<InternalWord> internalWords = new();
            foreach (Words word in words)
            {
                internalWords.Add(new InternalWord()
                {
                    IdLink = IdToLinkDict.FirstOrDefault(f => f.Value == word.Source).Key,
                    Words = word.WordList
                });
            }

            return internalWords;
        }

        private void MapLinkToId(string[] links)
        {
            int i = 0;
            foreach (string link in links)
            {
                IdToLinkDict[i] = link;
                i++;
            }
        }
    }
}
