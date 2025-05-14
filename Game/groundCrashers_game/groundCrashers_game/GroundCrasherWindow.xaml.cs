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
using Newtonsoft.Json;
using System.IO;


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
            Button clicked = sender as Button;
            string name = clicked.Content.ToString();


            // path to json
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "JsonData", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);


            // check if file exists
            if (!File.Exists(jsonPath))
            {
                MessageBox.Show("JSON file not found!");
                return;
            }

            // read json
            string json = File.ReadAllText(jsonPath);
            List<Creature> creatures = JsonConvert.DeserializeObject<List<Creature>>(json);

            // selected creature
            Creature selected = creatures.FirstOrDefault(c => c.name == name);

            if (selected != null)
            {
                GroundCrasherName.Text = selected.name;
                GroundCrasherType.Text = selected.primary_type;
                GroundCrasherHP.Text = selected.stats.hp.ToString();
                GroundCrasherAttack.Text = selected.stats.attack.ToString();
                GroundCrasherDefense.Text = selected.stats.defense.ToString();

                MessageBox.Show($"You selected {selected.name}!");
            }
            else
            {
                MessageBox.Show($"Could not find creature: {name}");
            }
        }


    }
}
