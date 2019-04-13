using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SyndicationFeed.Client.View
{
    public partial class UserManagementWindow : Window
    {
        public UserManagementWindow()
        {
            InitializeComponent();
            Loaded += (o, args) => { MinHeight = ActualHeight; MaxHeight = ActualHeight; };
            SetBinding(PasswordProperty, new Binding("Password") { Mode = BindingMode.TwoWay });
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register(
                "Password", typeof(string), typeof(UserManagementWindow),
                    new FrameworkPropertyMetadata(OnPasswordChangedStatic));

        bool passwordIsChangingByUser = false;
        static void OnPasswordChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            var self = (UserManagementWindow)d;
            if (!self.passwordIsChangingByUser)
                self.PasswordInput.Password = self.Password;
        }

        void OnPasswordChangedByUser(object sender, RoutedEventArgs e)
        {
            try
            {
                passwordIsChangingByUser = true;
                Password = PasswordInput.Password;
            }
            finally
            {
                passwordIsChangingByUser = false;
            }
        }
    }
}
