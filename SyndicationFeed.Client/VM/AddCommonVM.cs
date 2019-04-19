using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyndicationFeed.Client.VM
{
    abstract class AddCommonVM : VM
    {
        public AddCommonVM()
        {
            CheckAndAddCommand = new SimpleCommand(OnCheckAndAdd);
            CloseCommand = new SimpleCommand(OnClose);
            Execution = executionLifetime.Task;
        }

        protected readonly TaskCompletionSource<long?> executionLifetime = new TaskCompletionSource<long?>();

        public Task<long?> Execution { get; }

        string error;
        public string Error
        {
            get => error;
            set => Set(ref error, value);
        }

        public SimpleCommand CheckAndAddCommand { get; }
        public SimpleCommand CloseCommand { get; }

        protected abstract void OnCheckAndAdd();

        void OnClose()
        {
            executionLifetime.SetResult(null);
        }
    }
}
