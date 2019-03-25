using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models
{
    public class CollectionWithFeeds : Collection
    {
        // using internal to prevent the field from showing up in web response
        internal List<FeedWithDownloadTime> Feeds;
    }
}
