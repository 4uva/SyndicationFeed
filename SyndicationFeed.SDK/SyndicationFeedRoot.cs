using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;

namespace SyndicationFeed.SDK
{
    public class SyndicationFeedRoot
    {
        readonly WebHelper helper;

        public SyndicationFeedRoot(Uri uri, int port)
        {
            helper = new WebHelper(uri, port);
        }

        public async Task<List<Collection>> GetAllCollections()
        {
            // GET collections
            return await helper.GetAsync<List<Collection>>("collections");
        }

        public async Task<Collection> GetCollection(long id)
        {
            // GET collections/{id}
            return await helper.GetAsync<Collection>($"collections/{id}");
        }

        public async Task<Collection> AddCollection(string name)
        {
            // PUT collections
            return await helper.PutAsync<string, Collection>("collections", name);
        }
    }
}
