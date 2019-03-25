using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class FeedExpander
    {
        public void Expand(FeedWithDownloadTime feed)
        {
            switch (feed.Type)
            {
                case FeedType.Rss:
                    rssExpander.Expand(feed);
                    break;
                case FeedType.Atom:
                    atomExpander.Expand(feed);
                    break;
                default:
                    throw new ArgumentException($"Unknown feed type: {feed.Type}");
            }
        }

        RssExpander rssExpander = new RssExpander();
        AtomExpander atomExpander = new AtomExpander();

        //public FeedExpander()
        //{
        //    expandByFeedType = new Dictionary<FeedType, Action<Feed>>()
        //    {
        //        [FeedType.Rss] = rssExpander.Expand,
        //        [FeedType.Atom] = atomExpander.Expand
        //    };
        //}
        //
        //public void Expand(Feed feed)
        //{
        //    if (!expandByFeedType.TryGetValue(feed.Type, out var concreteExpander))
        //        concreteExpander(feed);
        //    else
        //        throw new ArgumentException($"Unknown feed type: {feed.Type}");
        //}
        //
        //Dictionary<FeedType, Action<Feed>> expandByFeedType;
    }
}
