using groundCrashers_game.classes;
using System.Windows;

namespace groundCrashers_game
{
    public partial class MainWindow : Window
    {
        AudioPlayer player;

        public MainWindow()
        {
            InitializeComponent();

            player = new AudioPlayer();
            player.Stop();
            player.PlaySpecific("intro.wav",true);
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // false is that the game is started from the main menu (arcadeMode)
            GameWindow gameWindow = new GameWindow(false);
            gameWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveAccount.Active_id == 0)
            {
                LoginWindow LoginWindow = new LoginWindow();
                LoginWindow.Show();
                this.Close();
            }
            else
            {
                LevelMapWindow LevelMapWindow = new LevelMapWindow();
                LevelMapWindow.Show();
                this.Close();
            }

        }

        private void Portal_Click(object sender, RoutedEventArgs e)
        {
            espConnectionWindow espConnectionWindow = new espConnectionWindow();
            espConnectionWindow.Show();
        }
    }
}

