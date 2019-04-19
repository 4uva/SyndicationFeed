using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SyndicationFeed.Common.Models;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class AddFeedVM : AddCommonVM
    {
        public AddFeedVM(SdkCollection modelCollection)
        {
            this.modelCollection = modelCollection;
        }

        readonly SdkCollection modelCollection;

        FeedType type;
        public FeedType Type
        {
            get => type;
            set => Set(ref type, value);
        }

        public IEnumerable<FeedType> AllTypes => new[] { FeedType.Rss, FeedType.Atom };

        string uriString;
        public string UriString
        {
            get => uriString;
            set => Set(ref uriString, value);
        }

        protected async override void OnCheckAndAdd()
        {
            Error = null;
            if (!Uri.TryCreate(UriString, UriKind.Absolute, out Uri uri))
            {
                Error = "Cannot parse feed address";
                return;
            }

            try
            {
                var sdkFeed = await modelCollection.AddFeed(Type, uri);
                // finish execution
                executionLifetime.SetResult(sdkFeed.Id);
            }
            catch (Exception ex)
            {
                // TODO: catch concrete exception
                Error = "Couldn't add feed: " + ex.Message;
            }
        }
    }
}