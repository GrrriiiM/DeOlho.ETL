using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DeOlho.ETL.Sources
{
    public class HttpJsonSource : ISource<string>
    {
        readonly string _uri;
        readonly HttpClient _httpClient;
        public HttpJsonSource(HttpClient httpClient,  string uri)
        {
            this._uri = uri;
            this._httpClient = httpClient;
        }

        public async Task<string> Execute()
        {
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await this._httpClient.GetAsync(this._uri);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}