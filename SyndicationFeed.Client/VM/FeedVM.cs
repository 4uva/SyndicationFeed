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
            _ = Load(modelCollection, id);
        }

        long id;
        Feed feed;

        bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            private set => Set(ref isLoading, value);
        }

        string error;
        public string Error
        {
            get => error;
            set => Set(ref error, value);
        }

        IReadOnlyCollection<PublicationVM> publications;
        public IReadOnlyCollection<PublicationVM> Publications
        {
            get => publications;
            private set
            {
                if (Set(ref publications, value))
                    PublicationCount = publications?.Count ?? 0;
            }
        }

        int publicationCount;
        public int PublicationCount
        {
            get => publicationCount;
            private set => Set(ref publicationCount, value);
        }

        async Task Load(SdkCollection modelCollection, long id)
        {
            try
            {
                IsLoading = true;
                Error = null;
                Publications = null;
                feed = await modelCollection.GetFeed(id);
                if (feed.LoadFailureMessage != null)
                {
                    Error = feed.LoadFailureMessage;
                    Publications = null;
                }
                else
                {
                    Error = null;
                    Publications = feed.Publications.Select(p => new PublicationVM(p)).ToList();
                }
            }
            catch (Exception ex)
            {
                Error = "Cannot download feed";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RemoveItself(SdkCollection modelCollection)
        {
            await modelCollection.DeleteFeed(id);
        }
    }
}
