using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SyndicationFeed.Common.Models
{
    [DataContract] 
    public class Publication
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Summary { get; set; }

        [DataMember]
        public DateTime? PublishTime { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public Uri Link { get; set; }
    }
}
