using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Models
{

    /*
     *   Property          Sent over Web API   Stored in DB
     *  ---------------------------------------------------
     *         Id                  +                 + (PK)
     *        Type                 +                 +
     *    SourceAddress            +                 -
     *    Publications             +                 -
     *  LoadFailureMessage         +                 -
     *   LastDownloadTime          -                 -
     *      ValidTill              -                 -
     *  SourceAddressString        -                 +
     */

    [DataContract]
    public class FeedWithDownloadTime : Feed
    {
        // no [DataMember] to prevent the field from showing up
        // in web response
        public DateTime LastDownloadTime { get; set; }
        public DateTime ValidTill { get; set; }

        internal string SourceAddressString
        {
            get => SourceAddress?.ToString();
            set => SourceAddress = (value == null ? null : new Uri(value));
        }
    }
}
