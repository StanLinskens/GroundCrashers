using groundCrashers_game.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TBXusername_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderUsername.Visibility = string.IsNullOrEmpty(TBXusername.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void PBXpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceholderPassword.Visibility = string.IsNullOrEmpty(PBXpassword.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string username = TBXusername.Text;
            string password = PBXpassword.Password;

            AccountManager.AddAccount(username, password);

            MessageBox.Show("Account created!");
        }
    }
}
