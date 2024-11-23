using System.Net.Http;

namespace Contracts.IServices
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);

        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}
