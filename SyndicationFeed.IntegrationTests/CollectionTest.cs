using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using SyndicationFeed.SDK;
using Xunit;

namespace SyndicationFeed.IntegrationTests
{
    // based on articles
    // https://fullstackmark.com/post/20/painless-integration-testing-with-aspnet-core-web-api
    // and
    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-2.2
    public class CollectionTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        SyndicationFeedRoot root;

        public CollectionTest(WebApplicationFactory<Startup> factory)
        {
            var client = factory.CreateClient();
            root = new SyndicationFeedRoot(client);
        }

        [Fact]
        public async Task TestSingleCollectionAdding()
        {
            var name = "Test";
            var collection = await root.AddCollection(name);
            Assert.Equal(name, collection.Name);
        }

        [Fact]
        public async Task TestCollectionId()
        {
            var name = "Test 2";
            var idBefore = await root.GetCollectionIds();
            var newCollection = await root.AddCollection(name);
            var idAfter = await root.GetCollectionIds();
            Assert.Contains(newCollection.Id, idAfter);
            Assert.DoesNotContain(newCollection.Id, idBefore);
        }

        [Fact]
        public async Task TestCollectionDelete()
        {
            var coll1 = await root.AddCollection("Sample 1");
            var id1 = coll1.Id;

            var coll2 = await root.AddCollection("Sample 2");
            var id2 = coll2.Id;

            await root.DeleteCollection(id1);

            var ids = await root.GetCollectionIds();

            Assert.Contains(id2, ids);
            Assert.DoesNotContain(id1, ids);
        }

        [Fact]
        public async Task TestCollectionGetAll()
        {
            for (int i = 0; i < 5; i++)
                await root.AddCollection($"Sample {i}");

            List<SdkCollection> collections = await root.GetAllCollections();

            List<long> ids = await root.GetCollectionIds();

            foreach (long id in ids)
            {
                SdkCollection collection = await root.GetCollection(id);
                Assert.Contains(collections, c => c.Id == collection.Id && c.Name == collection.Name);
            }
        }

        [Fact]
        public async Task TestWrongIdAccessThrows()
        {
            var ids = await root.GetCollectionIds();
            var nonexistingId = ids.Count > 0 ? ids.Max() + 1 : 0;
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                async () => await root.GetCollection(nonexistingId));
        }
    }
}
