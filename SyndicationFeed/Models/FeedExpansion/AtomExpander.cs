using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SyndicationFeed.Models.FeedCache;

namespace SyndicationFeed.Models.FeedExpansion
{
    public class AtomExpander : RssAndAtomExpanderCommon
    {
        public AtomExpander(Cache cache) : base(cache)
        {
        }
    }
}
