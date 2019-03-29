using System;
using System.Collections.Generic;
using System.Linq;
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
    public class FeedTest : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
    {
        SyndicationFeedRoot root;
        SdkCollection collection;

        public FeedTest(WebApplicationFactory<Startup> factory)
        {
            var client = factory.CreateClient();
            root = new SyndicationFeedRoot(client);
        }

        public async Task InitializeAsync()
        {
            collection = await root.AddCollection("Test collection");
        }

        public async Task DisposeAsync()
        {
            await root.DeleteCollection(collection.Id);
        }

        static bool PublicationsEqual(Publication p1, Publication p2)
        {
            return p1.Link == p2.Link &&
                   p1.Summary == p2.Summary &&
                   p1.Title == p2.Title &&
                   p1.Content == p2.Content;
        }

        static readonly Uri RssFeedAddress = new Uri("https://www.feedforall.com/sample.xml");
        static readonly Uri AtomFeedAddress = new Uri("https://en.blog.wordpress.com/feed/atom/");

        [Fact]
        public async Task TestSingleRssFeedAdding()
        {
            var feed = await collection.AddFeed(FeedType.Rss, RssFeedAddress);
            Assert.Equal(FeedType.Rss, feed.Type);
            Assert.Equal(RssFeedAddress, feed.SourceAddress);
        }

        [Fact]
        public async Task TestSingleAtomFeedAdding()
        {
            var feed = await collection.AddFeed(FeedType.Atom, AtomFeedAddress);
            Assert.Equal(FeedType.Atom, feed.Type);
            Assert.Equal(AtomFeedAddress, feed.SourceAddress);
        }

        [Fact]
        public async Task TestTotalFeed()
        {
            var localCollection = await root.AddCollection("Collection 2");
            var feed1 = await collection.AddFeed(FeedType.Rss, RssFeedAddress);
            var feed2 = await collection.AddFeed(FeedType.Atom, AtomFeedAddress);
            var totalFeed = await collection.GetTotalFeed();

            Assert.Equal(FeedType.Virtual, totalFeed.Type);
            foreach (var publication in feed1.Publications)
                Assert.Contains(totalFeed.Publications, p => PublicationsEqual(p, publication));
            foreach (var publication in feed2.Publications)
                Assert.Contains(totalFeed.Publications, p => PublicationsEqual(p, publication));
        }

        //[Fact]
        //public async Task TestCollectionId()
        //{
        //    var name = "Test 2";
        //    var idBefore = await root.GetCollectionIds();
        //    var newCollection = await root.AddCollection(name);
        //    var idAfter = await root.GetCollectionIds();
        //    Assert.Contains(newCollection.Id, idAfter);
        //    Assert.DoesNotContain(newCollection.Id, idBefore);
        //}
        //
        //[Fact]
        //public async Task TestCollectionDelete()
        //{
        //    var coll1 = await root.AddCollection("Sample 1");
        //    var id1 = coll1.Id;
        //
        //    var coll2 = await root.AddCollection("Sample 2");
        //    var id2 = coll2.Id;
        //
        //    await root.DeleteCollection(id1);
        //
        //    var ids = await root.GetCollectionIds();
        //
        //    Assert.Contains(id2, ids);
        //    Assert.DoesNotContain(id1, ids);
        //}
        //
        //[Fact]
        //public async Task TestCollectionGetAll()
        //{
        //    for (int i = 0; i < 5; i++)
        //        await root.AddCollection($"Sample {i}");
        //
        //    List<SdkCollection> collections = await root.GetAllCollections();
        //
        //    List<long> ids = await root.GetCollectionIds();
        //
        //    foreach (long id in ids)
        //    {
        //        SdkCollection collection = await root.GetCollection(id);
        //        Assert.Contains(collections, c => c.Id == collection.Id && c.Name == collection.Name);
        //    }
        //}
        //
        //[Fact]
        //public async Task TestWrongIdAccessThrows()
        //{
        //    var ids = await root.GetCollectionIds();
        //    var nonexistingId = ids.Count > 0 ? ids.Max() + 1 : 0;
        //    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
        //        async () => await root.GetCollection(nonexistingId));
        //}
    }
}
