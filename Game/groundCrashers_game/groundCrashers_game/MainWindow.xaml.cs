using groundCrashers_game.classes;
using System.Windows;

namespace groundCrashers_game
{
    public partial class MainWindow : Window
    {
        LoginWindow LoginWindow;

        public MainWindow()
        {
            InitializeComponent();

            LoginWindow = new LoginWindow(this);

            // music
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("intro.wav",true);

            if(ActiveAccount.Active_id == 0) logOut.Visibility = Visibility.Collapsed;
            else logOut.Visibility = Visibility.Visible;

        }

        /// <summary>
        /// start the normal game (non story game)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // false is that the game is started from the main menu (arcadeMode)
            GameWindow gameWindow = new GameWindow(false);
            gameWindow.Show();
            LoginWindow.Close(); // close the login window if it is open
            this.Close();
        }

        /// <summary>
        /// leave the game button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// to open the login window so the user can play the story mode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAccount.Active_id == 0)
            {
                LoginWindow.Show();
            }
            else
            {
                LevelMapWindow LevelMapWindow = new LevelMapWindow();
                LevelMapWindow.Show();
                this.Close();
            }

        }

        /// <summary>
        /// when the user clicks on the portal button, it will open the esp connection window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Portal_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.Close();
            espConnectionWindow espConnectionWindow = new espConnectionWindow();
            espConnectionWindow.Show();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow.Close();
            ActiveAccount.Active_id = 0; // reset the active account id to 0
            logOut.Visibility = Visibility.Collapsed; // hide the logout button
        }
    }
}

