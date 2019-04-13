using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    class UserVM : VM
    {
        public UserVM(UserManagement userManagement)
        {
            this.userManagement = userManagement;
            LoginCommand = new SimpleCommand(OnLogin);
            LogoutCommand = new SimpleCommand(OnLogout);
        }

        readonly UserManagement userManagement;

        string userName;
        public string UserName
        {
            get => userName;
            set => Set(ref userName, value);
        }

        string password;
        public string Password
        {
            get => password;
            set => Set(ref password, value);
        }

        public SimpleCommand LoginCommand { get; }
        public SimpleCommand LogoutCommand { get; }

        async void OnLogin()
        {
            try
            {
                var root = userManagement.Login(UserName, Password);
                // TODO: SDK doesn't report login status, needs to be corrected
                await root.GetCollectionIds();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Problem: {ex.Message}");
            }
        }

        void OnLogout()
        {
            MessageBox.Show($"Logging out");
        }
    }
}
