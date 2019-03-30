using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationFeed.Models.FeedCache
{
    public class Cache
    {
        const int cleanupThreshold = 1000;
        const int cleanupAmount = 100;

        object mutex = new object();

        Dictionary<Uri, FeedWithDownloadTime> feedsByUri =
            new Dictionary<Uri, FeedWithDownloadTime>();

        public (FeedWithDownloadTime feed, bool needRedownload) TryGetFeed(Uri uri)
        {
            lock (mutex)
            {
                if (feedsByUri.TryGetValue(uri, out var feed))
                {
                    bool needRedownload = feed.ValidTill < DateTime.UtcNow;
                    return (feed, needRedownload);
                }
                else
                {
                    return (null, true);
                }
            }
        }

        public void CacheFeed(FeedWithDownloadTime feed)
        {
            var uri = feed.SourceAddress;
            var cloneFeed = new FeedWithDownloadTime()
            {
                Id = feed.Id,
                Publications = feed.Publications,
                SourceAddress = uri,
                Type = feed.Type,
                LastDownloadTime = feed.LastDownloadTime,
                ValidTill = feed.ValidTill
            };
            lock (mutex)
            {
                feedsByUri[uri] = cloneFeed;
                if (feedsByUri.Count >= cleanupThreshold)
                    RemoveMostObsolete(cleanupAmount);
            }
        }

        // called under lock
        void RemoveMostObsolete(int cleanupAmount)
        {
            // sort by validity time
            var obsoleteFeeds =
                feedsByUri.Values.OrderBy(feed => feed.ValidTill).Take(cleanupAmount).ToList();
            foreach (var feed in obsoleteFeeds)
                feedsByUri.Remove(feed.SourceAddress);
        }

        public void UncacheFeed(FeedWithDownloadTime feed)
        {
            var uri = feed.SourceAddress;
            lock (mutex)
            {
                feedsByUri.Remove(uri);
            }
        }
    }
}
