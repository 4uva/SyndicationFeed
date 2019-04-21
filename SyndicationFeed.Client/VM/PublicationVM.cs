using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.Client.VM
{
    class PublicationVM : VM
    {
        public PublicationVM(Publication publication)
        {
            this.publication = publication;
        }

        readonly Publication publication;

        public string Id => publication.Id;
        public string Title => publication.Title;
        public string Summary => publication.Summary;
        public DateTime? PublishTime => publication.PublishTime;
        public string Content => publication.Content;
        public Uri Link => publication.Link;
    }
}
