using System;
using System.Collections.Generic;

namespace SyndicationFeed.Common.Models
{
    public class Feed
    {
        public long Id { get; set; }
        public FeedType Type { get; set; }
        public Uri SourceAddress { get; set; }
        public List<Publication> Publications { get; set; }
        public string LoadFailureMessage { get; set; }
    }
}