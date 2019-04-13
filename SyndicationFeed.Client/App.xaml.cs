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
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // TODO: don't hardcode url + port?
            var uri = new Uri("https://localhost");
            var port = 44301;
            var userManagement = new UserManagement(uri, port);
            UserVM userVM = new UserVM(userManagement);

            UserManagementWindow authWindow = new UserManagementWindow()
            {
                DataContext = userVM
            };

            authWindow.Show();
        }
    }
}
