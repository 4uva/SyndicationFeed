using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.SDK
{
    public class SyndicationFeedRoot
    {
        readonly RestHelper helper;

        public SyndicationFeedRoot(Uri uri, int port)
        {
            helper = new RestHelper(uri, port);
        }

        public async Task<List<SdkCollection>> GetAllCollections()
        {
            // GET collections
            var list = await helper.GetAsync<List<SdkCollection>>("collections");
            foreach (var coll in list)
                coll.helper = helper;
            return list;
        }

        public async Task<SdkCollection> GetCollection(long id)
        {
            // GET collections/{id}
            var coll = await helper.GetAsync<SdkCollection>($"collections/{id}");
            coll.helper = helper;
            return coll;
        }

        public async Task<SdkCollection> AddCollection(string name)
        {
            // PUT collections
            var coll = await helper.PostAsync<string, SdkCollection>("collections", name);
            coll.helper = helper;
            return coll;
        }

        public async Task DeleteCollection(long id)
        {
            // DELETE collections/1
            await helper.DeleteAsync($"collections/{id}");
        }

        public async Task<List<long>> GetCollectionIds()
        {
            // GET collections/ids
            return await helper.GetAsync<List<long>>("collections/ids");
        }
    }
}
