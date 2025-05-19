using groundCrashers_game.classes;
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

            var player = new Actor("test_Player", true);
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
    }
}

