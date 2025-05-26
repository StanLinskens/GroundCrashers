using groundCrashers_game.classes;
using System.Windows;

namespace groundCrashers_game
{
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Map_Click(object sender, RoutedEventArgs e)
        {
            LevelMapWindow LevelMapWindow = new LevelMapWindow();
            LevelMapWindow.Show();
            this.Close();
        }

        private void Portal_Click(object sender, RoutedEventArgs e)
        {
            espConnectionWindow espConnectionWindow = new espConnectionWindow();
            espConnectionWindow.Show();
        }
    }
}

