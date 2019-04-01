using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.FeedCache;
using SyndicationFeed.Models.FeedExpansion;
using SyndicationFeed.Models.Storage.EF;

namespace SyndicationFeed.Models.Storage
{
    public class Repository
    {
        public Repository(FeedsContext context, Cache cache)
        {
            this.context = context;
            feedExpander = new FeedExpander(cache);
        }

        FeedExpander feedExpander;
        FeedsContext context;

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync() =>
            await context.Collections.Include(c => c.Feeds).ToListAsync();

        // https://stackoverflow.com/a/40360633/10243782
        // cannot use FindAsync
        async Task<CollectionWithFeeds> TryFindCollectionInternalAsync(long id) =>
            await context.Collections.Include(c => c.Feeds)
                                     .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Collection> TryFindCollectionAsync(long id) =>
            await TryFindCollectionInternalAsync(id);

        public async Task<Collection> AddNewCollectionAsync(string name)
        {
            var coll = new CollectionWithFeeds()
            {
                Name = name,
                Feeds = new List<FeedWithDownloadTime>()
            };
            await context.AddAsync(coll);
            await context.SaveChangesAsync();
            return coll;
        }

        public async Task<bool> TryRemoveCollectionAsync(long id)
        {
            var coll = await TryFindCollectionInternalAsync(id);
            if (coll == null)
                return false; // was not there
            context.Collections.Remove(coll);
            await context.SaveChangesAsync();
            return true; // true if found, false if not
        }

        public async Task<IEnumerable<Feed>> TryFindFeedsAsync(long collid)
        {
            var coll = await TryFindCollectionInternalAsync(collid);
            if (coll != null)
            {
                var feeds = coll.Feeds;
                var feedExpandTasks = new List<Task>();
                foreach (var feed in feeds) // simultaneously
                    feedExpandTasks.Add(feedExpander.ExpandAsync(feed));
                await Task.WhenAll(feedExpandTasks);
                return feeds;
            }
            else
            {
                return null;
            }
        }

        public async Task<Feed> TryFindFeedAsync(long collid, long id)
        {
            var coll = await TryFindCollectionInternalAsync(collid);
            if (coll != null)
            {
                var feed = coll.Feeds.SingleOrDefault(f => f.Id == id);
                if (feed != null)
                    await feedExpander.ExpandAsync(feed);
                return feed;
            }
            else
            {
                return null;
            }
        }

        public async Task<Feed> AddNewFeedAsync(long collid, FeedType type, Uri uri)
        {
            var coll = await TryFindCollectionInternalAsync(collid); 
            if (coll == null)
                return null;
            var newFeed = new FeedWithDownloadTime()
            {
                Type = type,
                SourceAddress = uri,
                Publications = new List<Publication>(),
                LastDownloadTime = DateTime.MinValue,
                ValidTill = DateTime.MinValue,
                LoadFailureMessage = null
            };
            await feedExpander.ExpandAsync(newFeed);
            coll.Feeds.Add(newFeed);
            await context.SaveChangesAsync();
            return newFeed;
        }

        public async Task<bool> TryRemoveFeedAsync(long collid, long id)
        {
            var coll = await TryFindCollectionInternalAsync(collid);
            if (coll == null)
                return false;

            var feed = coll.Feeds.SingleOrDefault(f => f.Id == id);
            var result = coll.Feeds.Remove(feed);
            if (result) // found
                await context.SaveChangesAsync();
            return result; // true if found, false if not
        }
    }
}
