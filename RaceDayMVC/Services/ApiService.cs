using System.Net.Http;
using System.Threading.Tasks;

namespace RacedayMvc.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _clientFactory;

        public ApiService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            var client = _clientFactory.CreateClient("RaceDayApi");
            return await client.GetAsync(endpoint);
        }

        public async Task<HttpResponseMessage> PostAsync(string endpoint, HttpContent content)
        {
            var client = _clientFactory.CreateClient("RaceDayApi");
            return await client.PostAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> PutAsync(string endpoint, HttpContent content)
        {
            var client = _clientFactory.CreateClient("RaceDayApi");
            return await client.PutAsync(endpoint, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            var client = _clientFactory.CreateClient("RaceDayApi");
            return await client.DeleteAsync(endpoint);
        }
    }
}