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
            AddFeedCommand = new SimpleCommand(OnAddFeed);
            RemoveCurrentFeedCommand = new SimpleCommand(OnRemoveCurrentFeed) { AllowExecute = false };
        }

        public string Name => modelCollection.Name;
        public long Id => modelCollection.Id;

        public SimpleCommand AddFeedCommand { get; }
        public SimpleCommand RemoveCurrentFeedCommand { get; }

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

        FeedVM currentFeed;
        public FeedVM CurrentFeed
        {
            get => currentFeed;
            set
            {
                if (Set(ref currentFeed, value))
                    RemoveCurrentFeedCommand.AllowExecute = (value != null);
            }
        }

        AddFeedVM addFeed;
        public AddFeedVM AddFeed
        {
            get => addFeed;
            set => Set(ref addFeed, value);
        }

        async void OnAddFeed()
        {
            try
            {
                AddFeedCommand.AllowExecute = false;
                AddFeed = new AddFeedVM(modelCollection);
                long? id = await AddFeed.Execution;
                if (id != null)
                {
                    var newFeed = new FeedVM(modelCollection, id.Value);
                    Feeds.Add(newFeed);
                    CurrentFeed = newFeed;
                }
            }
            finally
            {
                AddFeed = null;
                AddFeedCommand.AllowExecute = true;
            }
        }

        async void OnRemoveCurrentFeed()
        {
            if (CurrentFeed == null)
                return;

            try
            {
                RemoveCurrentFeedCommand.AllowExecute = false;
                await CurrentFeed.RemoveItself(modelCollection);
                var feedToBeDeleted = CurrentFeed;
                CurrentFeed = null;
                Feeds.Remove(feedToBeDeleted);
            }
            finally
            {
                RemoveCurrentFeedCommand.AllowExecute = (CurrentFeed != null);
            }
        }

        SdkCollection modelCollection;

        public async Task RemoveItself(SyndicationFeedRoot root)
        {
            await root.DeleteCollection(modelCollection.Id);
        }
    }
}
