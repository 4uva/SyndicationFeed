using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class FeedExpander
    {
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

        RssExpander rssExpander = new RssExpander();
        AtomExpander atomExpander = new AtomExpander();
    }
}
