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

        SdkCollection modelCollection;
    }
}
