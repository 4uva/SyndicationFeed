using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.FeedExpansion;

namespace SyndicationFeed.Models.Storage
{
    public class Repository
    {
        List<CollectionWithFeeds> collections = new List<CollectionWithFeeds>();
        long maxId = 0; 

        FeedExpander feedExpander = new FeedExpander();

        public IEnumerable<Collection> GetAllCollections() => collections;

        CollectionWithFeeds TryFindCollectionInternal(long id)
        {
            return collections.SingleOrDefault(i => i.Id == id);
        }

        public Collection TryFindCollection(long id) =>
            TryFindCollectionInternal(id);

        public Collection AddNewCollection(string name)
        {
            var coll = new CollectionWithFeeds()
            {
                Id = maxId,
                Name = name,
                Feeds = new List<FeedWithDownloadTime>()
            };
            collections.Add(coll);
            maxId++;
            return coll;
        }

        public bool TryRemoveCollection(long id)
        {
            var coll = TryFindCollectionInternal(id);
            return collections.Remove(coll); // true if found, false if not
        }

        public async Task<IEnumerable<Feed>> TryFindFeedsAsync(long collid)
        {
            var coll = TryFindCollectionInternal(collid);
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
            var coll = TryFindCollectionInternal(collid);
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
            var coll = TryFindCollectionInternal(collid); 
            if (coll == null)
                return null;
            var id = 0L; // long
            if (coll.Feeds.Count > 0)
                id = coll.Feeds.Max(feed => feed.Id) + 1; 
            var newFeed = new FeedWithDownloadTime()
            {
                Id = id,
                Type = type,
                SourceAddress = uri,
                Publications = new List<Publication>(),
                LastDownloadTime = DateTime.MinValue,
                ValidTill = DateTime.MinValue,
                LoadFailureMessage = null
            };
            await feedExpander.ExpandAsync(newFeed);
            coll.Feeds.Add(newFeed);
            return newFeed;
        }

        public bool TryRemoveFeed(long collid, long id)
        {
            var coll = TryFindCollectionInternal(collid);
            if (coll == null)
                return false;

            var feed = coll.Feeds.SingleOrDefault(f => f.Id == id);
            return coll.Feeds.Remove(feed); // true if found, false if not
        }
    }
}
