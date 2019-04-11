using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace SyndicationFeed.SDK
{
    // implemented after the official Microsoft's example:
    // https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
    class RestHelper
    {
        // TODO: implement authentication using DelegatingHandler
        // (see https://docs.microsoft.com/en-us/aspnet/web-api/overview/advanced/httpclient-message-handlers)
        HttpClient client;
        Func<Task> performAuthentication;

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

        public void SetupAuthorization<T>(string webApiAddress, T loginInfo)
        {
            performAuthentication =
                () => Authenticate(webApiAddress, loginInfo);
        }

        public void DropAuthorization()
        {
            performAuthentication = null;
        }

        async Task Authenticate<T>(string webApiAddress, T loginInfo)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = null;
                string token = await PostNoRedirectAsync<T>(
                    webApiAddress, loginInfo, useAuth: false);
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationException("Cannot log in", ex);
            }
        }

        async Task<HttpResponseMessage> WithAuth(
            Func<Task<HttpResponseMessage>> process, bool useAuth)
        {
            HttpResponseMessage response = await process();
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && useAuth)
            {
                if (performAuthentication == null)
                    throw new InvalidOperationException("No authorization is set up");
                // perform authorization and repeat
                await performAuthentication();
                response = await process();
            }
            return response;
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

        public async Task<T> GetAsync<T>(
            string webApiAddress,
            string wrongParameterNameOn404 = null,
            bool useAuth = true)
        {
            HttpResponseMessage response =
                await WithAuth(() => client.GetAsync(webApiAddress), useAuth);
            await CheckResponse(response, wrongParameterNameOn404);
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<string> PostNoRedirectAsync(
            string webApiAddress, bool useAuth = true)
        {
            HttpResponseMessage sendResponse =
                await WithAuth(() => client.PostAsync(webApiAddress, null), useAuth);
            sendResponse.EnsureSuccessStatusCode();
            return await sendResponse.Content.ReadAsAsync<string>();
        }

        public async Task<string> PostNoRedirectAsync<TSend>(
            string webApiAddress, TSend obj, bool useAuth = true)
        {
            HttpResponseMessage sendResponse =
                await WithAuth(() => client.PostAsJsonAsync(webApiAddress, obj), useAuth);
            sendResponse.EnsureSuccessStatusCode();
            return await sendResponse.Content.ReadAsAsync<string>();
        }

        public async Task<TReceive> PostAsync<TSend, TReceive>(
            string webApiAddress, TSend obj, bool useAuth = true)
        {
            HttpResponseMessage sendResponse =
                await WithAuth(() => client.PostAsJsonAsync(webApiAddress, obj), useAuth);
            sendResponse.EnsureSuccessStatusCode();

            var receiveAddress = sendResponse.Headers.Location.ToString();
            HttpResponseMessage receiveResponse =
                await WithAuth(() => client.GetAsync(receiveAddress), useAuth);
            if (receiveResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new InvalidOperationException("object deleted before fetching");
            receiveResponse.EnsureSuccessStatusCode();
            return await receiveResponse.Content.ReadAsAsync<TReceive>();
        }

        public async Task DeleteAsync(
            string webApiAddress, string wrongParameterNameOn404 = null, bool useAuth = true)
        {
            HttpResponseMessage response =
                await WithAuth(() => client.DeleteAsync(webApiAddress), useAuth);
            await CheckResponse(response, wrongParameterNameOn404);
        }
    }
}
