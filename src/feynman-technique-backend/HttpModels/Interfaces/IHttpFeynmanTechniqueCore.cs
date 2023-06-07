using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels.Interfaces
{
    public interface IHttpFeynmanTechniqueCore
    {
        public Uri PrepareAddress(string endpoint);
        public RestRequest? PrepareRequest<E>(Uri uri, Method method, List<E>? entity = null) where E : class;
    }
}
