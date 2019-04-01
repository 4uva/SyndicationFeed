using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SyndicationFeed.Common.Models
{
    [DataContract]
    public class Feed
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public FeedType Type { get; set; }

        [DataMember]
        public Uri SourceAddress { get; set; }

        [DataMember]
        public List<Publication> Publications { get; set; }

        [DataMember]
        public string LoadFailureMessage { get; set; }
    }
}