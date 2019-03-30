using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.SDK
{
    public class SdkCollection : Collection
    {
        internal RestHelper helper;

        string AddressPrefix => $"collections/{Id}/feeds";

        public async Task<Feed> GetTotalFeed()
        {
            // GET collections/1/feeds/all
            return await helper.GetAsync<Feed>($"{AddressPrefix}/all", nameof(Id));
        }

        public async Task<Feed> GetFeed(long feedId)
        {
            // GET collections/1/feeds/all
            return await helper.GetAsync<Feed>($"{AddressPrefix}/{feedId}", nameof(feedId));
        }

        public async Task<Feed> AddFeed(FeedType type, Uri uri)
        {
            if (type == FeedType.Virtual)
                throw new ArgumentOutOfRangeException(nameof(type), "Cannot add virtual feed");
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            // POST collections/1/feeds
            var newFeed = new Feed()
            {
                Type = type,
                SourceAddress = uri
            };
            return await helper.PostAsync<Feed, Feed>(AddressPrefix, newFeed);
        }

        public async Task DeleteFeed(long feedId)
        {
            // DELETE collections/1/feeds/1
            await helper.DeleteAsync($"{AddressPrefix}/{feedId}", nameof(feedId));
        }

        public async Task<List<long>> GetFeedIds()
        {
            // GET collections/1/feeds/ids
            return await helper.GetAsync<List<long>>($"{AddressPrefix}/ids", nameof(Id));
        }
    }
}
