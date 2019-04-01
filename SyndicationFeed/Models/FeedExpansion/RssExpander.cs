using System.Linq;
using System.Xml;
using SyndicationFeed.Models.FeedCache;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class RssExpander : RssAndAtomExpanderCommon
    {
        public RssExpander(Cache cache) : base(cache)
        {
        }
    }
}
