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

        public async Task<Feed> GetSyndicatedFeed()
        {
            // GET collections/1/feeds/all
            return await helper.GetAsync<Feed>($"{AddressPrefix}/all");
        }

        public async Task<Feed> GetFeed(long feedId)
        {
            // GET collections/1/feeds/all
            return await helper.GetAsync<Feed>($"{AddressPrefix}/{feedId}");
        }

        public async Task<Feed> AddFeed(FeedType type, Uri uri)
        {
            // PUT collections/1/feeds
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
            await helper.DeleteAsync($"{AddressPrefix}/{feedId}");
        }

        public async Task<List<long>> GetFeedIds()
        {
            // GET collections/1/feeds/ids
            return await helper.GetAsync<List<long>>($"{AddressPrefix}/ids");
        }
    }
}
