using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models
{
    /*
     *   Property    Sent over Web API   Stored in DB
     *  ------------------------------------------------
     *     Id               +                  + (PK)
     *    Name              +                  +
     *    Feeds             -                 nav
     *    User              -                 nav
     */

    [DataContract]
    public class CollectionWithFeeds : Collection
    {
        // not setting [DataMember] in order to avoid Feeds and User transfer
        // through the API
        public List<FeedWithDownloadTime> Feeds { get; set; }
        public IdentityUser User { get; set; }
    }
}
