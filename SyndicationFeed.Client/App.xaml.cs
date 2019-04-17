using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using SyndicationFeed.Client.View;
using SyndicationFeed.Client.VM;
using SyndicationFeed.SDK;

namespace SyndicationFeed.Client
{
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs args)
        {
            base.OnStartup(args);

            // TODO: don't hardcode url + port?
            var uri = new Uri("https://localhost");
            var port = 44301;
            var userManagement = new UserManagement(uri, port);
            userVM = new UserVM(userManagement);
            mainVM = new MainVM(userVM);
            MainWindow mainWindow = new MainWindow()
            {
                DataContext = mainVM
            };

            var tcs = new TaskCompletionSource<object>();
            mainWindow.Loaded += (e2, args2) => tcs.TrySetResult(null);
            mainWindow.Show();
            await tcs.Task;

            // needs to be called after main window is shown
            ShowAuthenticationDialog();
        }

        public void ShowAuthenticationDialog()
        {
            var authWindow = new UserManagementWindow()
            {
                DataContext = userVM,
                Owner = MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            authWindow.Show();
        }

        UserVM userVM;
        MainVM mainVM;
    }
}
