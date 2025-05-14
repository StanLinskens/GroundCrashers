using groundCrashers_game.classes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        private void Fight_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Bag_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GroundCrashers_Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroundCrasherWindow GroundCrasherWindow = new GroundCrasherWindow();
            GroundCrasherWindow.Show();
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}