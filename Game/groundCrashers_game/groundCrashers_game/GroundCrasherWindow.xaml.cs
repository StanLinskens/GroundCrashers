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
    /// <summary>
    /// Interaction logic for GroundCrasherWindow.xaml
    /// </summary>
    public partial class GroundCrasherWindow : Window
    {
        public GroundCrasherWindow()
        {
            InitializeComponent();
        }

        private void SelectPokemon_Click(object sender, RoutedEventArgs e)
        {
            // generate rondom number
            Random rnd = new Random();
            // get the button that has been selected
            Button clicked = sender as Button;
            // take the name and convert it to string
            string name = clicked.Content.ToString();
            // generate random number for stats (temporary)
            int Type = rnd.Next(1, 100);
            int HP = rnd.Next(1, 100);
            int Attack = rnd.Next(1, 100);
            int Defense = rnd.Next(1, 100);



            // Load data based on selected name (use a dictionary or database)
            GroundCrasherName.Text = name;
            GroundCrasherType.Text =  Type.ToString();
            GroundCrasherHP.Text = HP.ToString();
            GroundCrasherAttack.Text = Attack.ToString();
            GroundCrasherDefense.Text = Defense.ToString();

            // show messagebox to confirm
            MessageBox.Show($"You selected {name}!");
        }

    }
}
