using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SyndicationFeed.Client.VM
{
    class SimpleCommand : ICommand
    {
        public SimpleCommand(Action action)
        {
            this.action = action;
        }

        public bool AllowExecute
        {
            get => allowExecute;
            set
            {
                if (value != allowExecute)
                {
                    allowExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => allowExecute;

        public void Execute(object parameter)
        {
            if (allowExecute)
                action();
        }

        readonly Action action;
        bool allowExecute = true;
    }
}
