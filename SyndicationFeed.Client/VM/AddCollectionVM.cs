using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class AddCollectionVM : AddCommonVM
    {
        public AddCollectionVM(SyndicationFeedRoot root)
        {
            this.root = root;
        }

        readonly SyndicationFeedRoot root;

        string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        protected async override void OnCheckAndAdd()
        {
            Error = null;
            if (string.IsNullOrWhiteSpace(Name))
            {
                Error = "Name must be non-empty";
                return;
            }

            try
            {
                var sdkCollection = await root.AddCollection(Name);
                // finish execution
                executionLifetime.SetResult(sdkCollection.Id);
            }
            catch (Exception ex)
            {
                // TODO: catch concrete exception
                Error = "Couldn't add collection: " + ex.Message;
            }
        }
    }
}
