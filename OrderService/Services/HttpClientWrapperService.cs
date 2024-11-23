using Contracts.IServices;

namespace Services
{
    public class HttpClientWrapperService : IHttpClientService
    {
        private readonly HttpClient client;

        public HttpClientWrapperService(HttpClient client)
        {
            this.client = client;
        }

        public virtual Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return client.GetAsync(requestUri);
        }

        public virtual Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return client.PostAsync(requestUri, content);
        }
    }
}
