using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.SDK
{
    // TODO: implement IDisposable
    public class UserManagement
    {
        readonly RestHelper helper;

        public UserManagement(Uri uri, int port)
        {
            helper = new RestHelper(uri, port);
        }

        public UserManagement(HttpClient client)
        {
            helper = new RestHelper(client);
        }

        public SyndicationFeedRoot Login(string userName, string password)
        {
            var loginInfo = new LoginInfo()
            {
                UserName = userName,
                Password = password
            };
            helper.SetupAuthorization("auth/login", loginInfo);
            return new SyndicationFeedRoot(helper);
        }

        public void Logout()
        {
            helper.DropAuthorization();
        }

        public async Task Register(RegisterInfo info)
        {
            try
            {
                await helper.PostNoRedirectAsync(
                    "auth/register", info, useAuth: false);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException("Registration failed", ex);
            }
        }

        public async Task Unregister()
        {
            await helper.DeleteAsync(
                "auth/unregister", wrongParameterNameOn404: null, useAuth: true);
            helper.DropAuthorization();
        }
    }
}
