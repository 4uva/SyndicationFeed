using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;
using SyndicationFeed.Models.FeedCache;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class FeedExpander
    {
        public FeedExpander(Cache cache)
        {
            rssExpander = new RssExpander(cache);
            atomExpander = new AtomExpander(cache);
        }

        public async Task ExpandAsync(FeedWithDownloadTime feed)
        {
            switch (feed.Type)
            {
                case FeedType.Rss:
                    await rssExpander.ExpandAsync(feed);
                    break;
                case FeedType.Atom:
                    await atomExpander.ExpandAsync(feed);
                    break;
                default:
                    throw new ArgumentException($"Unknown feed type: {feed.Type}");
            }
        }

        RssExpander rssExpander;
        AtomExpander atomExpander;
    }
}
