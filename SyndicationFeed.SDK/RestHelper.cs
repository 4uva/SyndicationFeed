using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SyndicationFeed.SDK
{
    // implemented after the official Microsoft's example:
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    class RestHelper
    {
        HttpClient client;

        public RestHelper(Uri uri, int port)
        {
            var builder = new UriBuilder(uri)
            {
                Port = port,
                Path = "api/"
            };

            client = new HttpClient() { BaseAddress = builder.Uri };
            SetupClient();
        }

        public RestHelper(HttpClient client)
        {
            this.client = client;
            client.BaseAddress = new Uri(client.BaseAddress, "api/");
            SetupClient();
        }

        void SetupClient()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // checks if response is successful, for the 404 reply tries to extract error message
        async Task CheckResponse(HttpResponseMessage response, string wrongParameterNameOn404)
        {
            if (wrongParameterNameOn404 != null && response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                var mediaType = response.Content.Headers.ContentType.MediaType;
                string errMessage = "Id doesn't exist";
                if (mediaType == "application/json")
                {
                    try
                    {
                        errMessage = await response.Content.ReadAsAsync<string>();
                    }
                    catch
                    {
                        // couldn't deserialize error message, can be safely ignored
                    }
                }

                throw new ArgumentOutOfRangeException(wrongParameterNameOn404, errMessage);
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<T> GetAsync<T>(string webApiAddress, string wrongParameterNameOn404 = null)
        {
            HttpResponseMessage response = await client.GetAsync(webApiAddress);
            await CheckResponse(response, wrongParameterNameOn404);
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<TReceive> PostAsync<TSend, TReceive>(
            string webApiAddress, TSend obj)
        {
            HttpResponseMessage sendResponse = await client.PostAsJsonAsync(webApiAddress, obj);
            sendResponse.EnsureSuccessStatusCode();

            var receiveAddress = sendResponse.Headers.Location.ToString();
            HttpResponseMessage receiveResponse = await client.GetAsync(receiveAddress);
            if (receiveResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new InvalidOperationException("object deleted before fetching");
            receiveResponse.EnsureSuccessStatusCode();
            return await receiveResponse.Content.ReadAsAsync<TReceive>();
        }

        public async Task DeleteAsync(string webApiAddress, string wrongParameterNameOn404)
        {
            HttpResponseMessage response = await client.DeleteAsync(webApiAddress);
            await CheckResponse(response, wrongParameterNameOn404);
        }
    }
}
