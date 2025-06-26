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

        /// <summary>
        /// for the username
        /// when the text in the username textbox changes, this method will be called to show or hide the placeholder text  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TBXusername_TextChanged(object sender, TextChangedEventArgs e)
        {
            PlaceholderUsername.Visibility = string.IsNullOrEmpty(TBXusername.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// for the username
        /// when the text in the username textbox changes, this method will be called to show or hide the placeholder text  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PBXpassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PlaceholderPassword.Visibility = string.IsNullOrEmpty(PBXpassword.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// handle the event for loggin in
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = TBXusername.Text;
            string password = PBXpassword.Password;

            bool success = AccountManager.Login(username, password);

            if (success)
            {
                LevelMapWindow LevelMapWindow = new LevelMapWindow();
                LevelMapWindow.Show();
                this.Close(); // Close the login window after successful login
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// this method is called when the create account button is clicked
        /// to create a new account with the username and password entered in the textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            string username = TBXusername.Text;
            string password = PBXpassword.Password;

            if(username != "" || password != "")
            {
                AccountManager.AddAccount(username, password);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // Close the login window when going back to the main menu
        }
    }
}
