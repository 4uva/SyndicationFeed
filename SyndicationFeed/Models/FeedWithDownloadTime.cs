using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models
{
    public class FeedWithDownloadTime : Feed
    {
        // using internal to prevent the field from showing up in web response
        internal DateTime LastDownloadTime;
        internal DateTime ValidTill;
    }
}
