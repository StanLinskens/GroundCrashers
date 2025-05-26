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
using groundCrashers_game.classes;


namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for GroundCrasherWindow.xaml
    /// </summary>
    public partial class GroundCrasherWindow : Window
    {
        private List<Creature> loadedCreatures;

        private Manager _Manager;

        private GameWindow _gameWindow;

        public GroundCrasherWindow(Manager manager, GameWindow gameWindow)
        {
            InitializeComponent();
            _Manager = manager;
            _gameWindow = gameWindow;
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
                GroundCrasherType.Text = selected.primary_type.ToString();
                GroundCrasherElement.Text = selected.element.ToString();
                GroundCrasherHP.Text = selected.stats.hp.ToString();
                GroundCrasherAttack.Text = selected.stats.attack.ToString();
                GroundCrasherDefense.Text = selected.stats.defense.ToString();
                GroundCrasherSpeed.Text = selected.stats.speed.ToString();
                try {
                    creatureImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{selected.name}.png", UriKind.Absolute));
                    } 
                catch
                {
                    //MessageBox.Show($"Image not found for {selected.name}. Please check the image path.");
                    creatureImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }

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

                SelectedCreature = selected;
            }
            else
            {
                MessageBox.Show($"Could not find creature: {name}");
            }
        }

        public Creature SelectedCreature { get; private set; }

        private void LoadCreatureButtons()
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
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
                    Style = (Style)FindResource("DarkButton"),
                };

                btn.Click += SelectPokemon_Click;
                CreatureButtonPanel.Children.Add(btn);
            }
        }

        private void ConfirmGroundCrasher_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCreature != null)
            {
                string name = SelectedCreature.name;

                Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

                //_Manager.PrintActors();
                _Manager.AddPlayerCreatures(selected);

                // Tell GameWindow to update the logbox
                _gameWindow.RefreshLogBox();
            }
            else
            {
                _Manager.logs.Add("no creature selected added to team");
            }
        }

    }
}
