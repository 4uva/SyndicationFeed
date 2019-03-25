using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndicationFeed.Common.Models
{
    public class Publication
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public DateTime? PublishTime { get; set; }
        public string Content { get; set; }
        public Uri Link { get; set; }
    }
}
