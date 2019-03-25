using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SyndicationFeed.SDK
{
    class WebHelper
    {
        HttpClient client;

        public WebHelper(Uri uri, int port)
        {
            var builder = new UriBuilder(uri)
            {
                Port = port,
                Path = "api/"
            };
            client = new HttpClient() { BaseAddress = builder.Uri };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<T> GetAsync<T>(string webApiAddress)
        {
            HttpResponseMessage response = await client.GetAsync(webApiAddress);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<TReceive> PutAsync<TSend, TReceive>(
            string webApiAddress, TSend obj)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                webApiAddress, obj);
            response.EnsureSuccessStatusCode();

            return await GetAsync<TReceive>(response.Headers.Location.ToString());
        }
    }
}
