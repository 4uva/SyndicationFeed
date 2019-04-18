using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class CollectionVM : VM
    {
        public CollectionVM(SdkCollection modelCollection)
        {
            this.modelCollection = modelCollection ??
                throw new ArgumentNullException(nameof(modelCollection));
        }

        public string Name => modelCollection.Name;
        public long Id => modelCollection.Id;

        public async Task LoadFeeds()
        {
            if (Feeds != null)
                return; // already loaded
            var ids = await modelCollection.GetFeedIds();
            Feeds = ids.Select(id => new FeedVM(modelCollection, id)).ToList();
        }

        public void UnloadFeeds()
        {
            Feeds = null;
        }

        List<FeedVM> feeds;
        public List<FeedVM> Feeds
        {
            get => feeds;
            private set
            {
                if (Set(ref feeds, value))
                    FeedCount = value?.Count ?? 0;
            }
        }

        int feedCount;
        public int FeedCount
        {
            get => feedCount;
            private set => Set(ref feedCount, value);
        }

        SdkCollection modelCollection;

        public async Task RemoveItself(SyndicationFeedRoot root)
        {
            await root.DeleteCollection(modelCollection.Id);
        }
    }
}
