using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DeOlho.ETL.Sources
{
    public class HttpStreamSource : ISource<Stream>
    {
        readonly string _uri;
        readonly HttpClient _httpClient;
        public HttpStreamSource(HttpClient httpClient,  string uri)
        {
            this._uri = uri;
            this._httpClient = httpClient;
        }

        public async Task<Stream> Execute()
        {
            this._httpClient.DefaultRequestHeaders.Accept.Clear();
            this._httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await this._httpClient.GetAsync(this._uri);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }


}