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
        // list of loaded creatures
        private List<Creature> loadedCreatures;

        public GroundCrasherWindow()
        {
            InitializeComponent();
        }

        // when the window is loaded, load the creature buttons
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCreatureButtons();
        }

        // select pokemon when clicked on button
        private void SelectPokemon_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string name = clicked.Content.ToString();

            // get selected creature from the list
            Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

            // check if the creature was found
            if (selected != null)
            {
                // set the text of the labels to the selected creature's stats
                GroundCrasherName.Text = selected.name;
                GroundCrasherType.Text = selected.primary_type;
                GroundCrasherHP.Text = selected.stats.hp.ToString();
                GroundCrasherAttack.Text = selected.stats.attack.ToString();
                GroundCrasherDefense.Text = selected.stats.defense.ToString();

                // show message box with the selected creature's name
                MessageBox.Show($"You selected {selected.name}!");
            }
            else
            {
                // creature not found
                MessageBox.Show($"Could not find creature: {name}");
            }
        }

        // load the creature buttons from the json file
        private void LoadCreatureButtons()
        {
            // path to the json file
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "JsonData", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            // check if the file exists
            if (!File.Exists(jsonPath))
            {
                // show error message
                MessageBox.Show("Creature JSON file not found!");
                return;
            }

            // path to the json
            string json = File.ReadAllText(jsonPath);
            loadedCreatures = JsonConvert.DeserializeObject<List<Creature>>(json); // Store the list

            // for each loop for the buttons
            foreach (Creature creature in loadedCreatures)
            {
                //make new button
                Button btn = new Button
                {
                    Content = creature.name,
                    Margin = new Thickness(5),
                    Height = 40,
                    Background = Brushes.Lavender
                };

                // set the button's name to the creature's name
                btn.Click += SelectPokemon_Click;
                CreatureButtonPanel.Children.Add(btn);
            }
        }

    }
}
