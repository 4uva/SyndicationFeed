using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class AddCollectionVM : VM
    {
        public AddCollectionVM(SyndicationFeedRoot root)
        {
            this.root = root;
            CheckAndAddCommand = new SimpleCommand(OnCheckAndAdd);
            CloseCommand = new SimpleCommand(OnClose);
            Execution = executionLifetime.Task;
        }

        readonly SyndicationFeedRoot root;

        TaskCompletionSource<long?> executionLifetime = new TaskCompletionSource<long?>();

        public Task<long?> Execution { get; }

        string name;
        public string Name
        {
            get => name;
            set => Set(ref name, value);
        }

        string error;
        public string Error
        {
            get => error;
            set => Set(ref error, value);
        }

        public SimpleCommand CheckAndAddCommand { get; }
        public SimpleCommand CloseCommand { get; }

        async void OnCheckAndAdd()
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

        void OnClose()
        {
            executionLifetime.SetResult(null);
        }
    }
}
