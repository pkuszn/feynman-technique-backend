using RestSharp;

namespace FeynmanTechniqueBackend.HttpModels
{
    public abstract class HttpClient
    {
        public RestRequest? PrepareRequest(Uri uri, Method method, object? body = null)
        {
            if (string.IsNullOrEmpty(uri.AbsolutePath) || body == null)
            {
                return null;
            }

            RestRequest request = new(uri, method);

            if (body != null)
            {
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(body);
            }

            return request;
        }

        public RestRequest? PrepareRequest<E>(Uri uri, Method method, List<E>? entity = null)
            where E : class
        {
            if (string.IsNullOrEmpty(uri.AbsolutePath))
            {
                return null;
            }

            if ((entity?.Count ?? 0) == 0)
            {
                return null;
            }

            RestRequest request = new(uri, method);

            if (entity != null)
            {
                request.AddHeader("Content-Type", "application/json");
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(entity);
            }

            return request;
        }
    }
}

