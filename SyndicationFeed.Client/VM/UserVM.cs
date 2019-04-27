using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SyndicationFeed.Common.Models;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client.VM
{
    // TODO: auto logout on idle X minutes?
    class UserVM : VM
    {
        public UserVM(UserManagement userManagement)
        {
            this.userManagement = userManagement;
            LoginCommand = new SimpleCommand(OnLogin);
            LogoutCommand = new SimpleCommand(OnLogout) { AllowExecute = false };
            RegisterCommand = new SimpleCommand(OnRegister);
            UnregisterCommand = new SimpleCommand(OnRegister) { AllowExecute = false };
            Status = "Not logged in";
        }

        readonly UserManagement userManagement;
        SyndicationFeedRoot root;

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

        bool isAuthenticated = false;
        public bool IsAuthenticated
        {
            get => isAuthenticated;
            private set
            {
                if (Set(ref isAuthenticated, value))
                {
                    LogoutCommand.AllowExecute = isAuthenticated;
                    LoginCommand.AllowExecute = !isAuthenticated;
                    UnregisterCommand.AllowExecute = isAuthenticated;
                }
            }
        }

        string status;
        public string Status
        {
            get => status;
            private set => Set(ref status, value);
        }

        public SimpleCommand RegisterCommand { get; }
        public SimpleCommand UnregisterCommand { get; }
        public SimpleCommand LoginCommand { get; }
        public SimpleCommand LogoutCommand { get; }

        public SyndicationFeedRoot GetFeedRoot()
        {
            if (IsAuthenticated)
                return root;
            else
                throw new InvalidOperationException("Need to be authenticated");
        }

        // only if not logged in
        async void OnLogin()
        {
            await PerformLogin();
        }

        async Task PerformLogin()
        {
            IsAuthenticated = false;
            try
            {
                Status = "Logging in...";
                root = userManagement.Login(UserName, Password);
                // TODO: SDK doesn't report login status, needs to be corrected
                await root.GetCollectionIds();

                // here we are successfuly logged in
                IsAuthenticated = true;
                Status = "Logged in";
            }
            catch (Exception ex)
            {
                // TODO: catch specific exception
                // TODO: differentiate between wrong credentials and unavailable server
                Status = "Login failure";
                root = null;
            }
        }

        // only if logged in
        void OnLogout()
        {
            if (!IsAuthenticated)
            {
                Status = "Not logged in";
                return;
            }

            userManagement.Logout();
            IsAuthenticated = false;
            root = null;
            Status = "Logged out";
        }

        // always
        async void OnRegister()
        {
            if (IsAuthenticated)
                OnLogout();

            try
            {
                Status = "Registering...";
                await userManagement.Register(
                    new UserInfo() { UserName = UserName, Password = Password });
                Status = "Successfully registered, logging in...";
                await PerformLogin();
            }
            catch (Exception ex)
            {
                // TODO: catch specific exception
                // TODO: differentiate register failure and unavailable server
                Status = "Registration failure";
            }
        }

        // only if logged in
        async void OnUnregister()
        {
            if (!IsAuthenticated)
            {
                Status = "Not logged in";
                return;
            }

            try
            {
                Status = "Registering...";
                await userManagement.Unregister();
                Status = "Successfully unregistered";

                userManagement.Logout();
                IsAuthenticated = false;
                root = null;
            }
            catch (Exception ex)
            {
                // TODO: catch specific exception
                // TODO: differentiate unregister failure and unavailable server
                Status = "Unregistration failure";
            }
        }
    }
}
