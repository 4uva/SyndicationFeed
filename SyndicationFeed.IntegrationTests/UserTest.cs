using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SyndicationFeed.Common.Models;
using SyndicationFeed.SDK;
using Xunit;

namespace SyndicationFeed.IntegrationTests
{
    // based on articles
    // https://fullstackmark.com/post/20/painless-integration-testing-with-aspnet-core-web-api
    // and
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2
    public class UserTest
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        CustomWebApplicationFactory<Startup> factory;

        public UserTest(CustomWebApplicationFactory<Startup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task TestUnregisteredUserLogin()
        {
            var u1 = "unittest user 1";
            var p1 = "*";
            var client = factory.CreateClient();
            var mgmt = new UserManagement(client);
            var root = mgmt.Login(u1, p1);
            await Assert.ThrowsAsync<AuthenticationException>(
                async () => await root.GetAllCollections());
        }

        [Fact]
        public async Task TestCreateDeleteUser()
        {
            var u1 = "unittest user 1";
            var p1 = "*";
            var client = factory.CreateClient();
            var mgmt = new UserManagement(client);
            await mgmt.Register(
                new RegisterInfo() { UserName = u1, Password = p1 });
            var root = mgmt.Login(u1, p1);
            var colls = await root.GetAllCollections();
            Assert.Empty(colls);
            await mgmt.Unregister();
        }

        [Fact]
        public async Task TestDataIsolation()
        {
            var client = factory.CreateClient();
            var mgmt = new UserManagement(client);
            var u1 = "unittest user 1";
            var p1 = "*";
            await mgmt.Register(
                new RegisterInfo() { UserName = u1, Password = p1 });
            var u2 = "unittest user 2";
            var p2 = "*";
            await mgmt.Register(
                new RegisterInfo() { UserName = u2, Password = p2 });

            var rootOfUser1 = mgmt.Login(u1, p1);
            await rootOfUser1.AddCollection("test");

            SyndicationFeedRoot rootOfUser2 = mgmt.Login(u2, p2);
            var collsOfUser2 = await rootOfUser2.GetAllCollections();
            Assert.Empty(collsOfUser2);
            await mgmt.Unregister();

            var rootOfUser1New = mgmt.Login(u1, p1);
            var collsOfUser1 = await rootOfUser1New.GetAllCollections();
            Assert.Single(collsOfUser1);
            await mgmt.Unregister();
        }
    }
}
