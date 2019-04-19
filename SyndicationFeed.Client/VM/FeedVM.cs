using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class FeedVM : VM
    {
        public FeedVM(SdkCollection modelCollection, long id)
        {
            this.id = id;
            var t = Load(modelCollection, id);
        }

        long id;
        Feed feed;
        string Error;

        async Task Load(SdkCollection modelCollection, long id)
        {
            try
            {
                feed = await modelCollection.GetFeed(id);
            }
            catch (Exception ex)
            {
                Error = "Cannot download feed";
            }
        }

        public async Task RemoveItself(SdkCollection modelCollection)
        {
            await modelCollection.DeleteFeed(id);
        }
    }
}
