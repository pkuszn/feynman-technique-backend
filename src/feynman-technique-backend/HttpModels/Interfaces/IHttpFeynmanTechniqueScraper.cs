using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels.Interfaces
{
    public interface IHttpFeynmanTechniqueScraper
    {
        public Uri PrepareAddress(string endpoint);
        public RestRequest? PrepareRequest(Uri uri, Method method, object? body = null);
    }
}
