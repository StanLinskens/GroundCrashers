using groundCrashers_game.classes;
using System.Numerics;
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
            //// Safety check
            //if (Player == null || Computer == null ||
            //    Player.Creatures.Count == 0 || Computer.Creatures.Count == 0)
            //{
            //    MessageBox.Show("Player or Computer is missing creatures!");
            //    return;
            //}

            //// Get creatures
            //var playerCreature = Player.Creatures[0]; // or selected index
            //var computerCreature = Computer.Creatures[0];

            //// Get stats
            //int playerAttack = playerCreature.stats.attack;
            //int computerDefense = computerCreature.stats.defense;

            //// Calculate damage using Manager
            //var battleManager = new Manager();
            //int damageDealt = battleManager.Damage(playerAttack, computerDefense);

            //// Show result
            //MessageBox.Show($"{playerCreature.name} dealt {damageDealt} damage to {computerCreature.name}!");

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