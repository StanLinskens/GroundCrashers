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
        private List<Creature> loadedCreatures;

        public GroundCrasherWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCreatureButtons();
        }

        public void SelectPokemon_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string name = clicked.Content.ToString();

            Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

            if (selected != null)
            {
                GroundCrasherName.Text = selected.name;
                GroundCrasherType.Text = selected.primary_type;
                GroundCrasherElement.Text = selected.element;
                GroundCrasherHP.Text = selected.stats.hp.ToString();
                GroundCrasherAttack.Text = selected.stats.attack.ToString();
                GroundCrasherDefense.Text = selected.stats.defense.ToString();

                var creature = new Creature
                {
                    id = selected.id,
                    name = selected.name,
                    primary_type = selected.primary_type,
                    stats = new Stats
                    {
                        hp = selected.stats.hp,
                        attack = selected.stats.attack,
                        defense = selected.stats.defense,
                        speed = selected.stats.speed
                    },
                    element = selected.element,
                    ability = selected.ability,
                    description = selected.description
                };
            }
            else
            {
                MessageBox.Show($"Could not find creature: {name}");
            }
        }

        private void LoadCreatureButtons()
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "JsonData", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            if (!File.Exists(jsonPath))
            {
                MessageBox.Show("Creature JSON file not found!");
                return;
            }

            string json = File.ReadAllText(jsonPath);
            loadedCreatures = JsonConvert.DeserializeObject<List<Creature>>(json); // Store the list

            foreach (Creature creature in loadedCreatures)
            {
                Button btn = new Button
                {
                    Content = creature.name,
                    Margin = new Thickness(5),
                    Height = 40,
                    Background = Brushes.Lavender
                };

                btn.Click += SelectPokemon_Click;
                CreatureButtonPanel.Children.Add(btn);
            }
        }

    }
}
