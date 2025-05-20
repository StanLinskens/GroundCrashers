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

        public GroundCrasherWindow(Manager manager)
        {
            InitializeComponent();

            _Manager = manager;
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
                try {
                    creatureImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{selected.name}.png", UriKind.Absolute));
                    } catch
                {
                    MessageBox.Show($"Image not found for {selected.name}. Please check the image path.");
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
                string statsMessage = $"You have selected {SelectedCreature.name}!\n\n" +
                                      $"Element: {SelectedCreature.element}\n" +
                                      $"Type: {SelectedCreature.primary_type}\n" +
                                      $"HP: {SelectedCreature.stats.hp}\n" +
                                      $"Attack: {SelectedCreature.stats.attack}\n" +
                                      $"Defense: {SelectedCreature.stats.defense}\n" +
                                      $"Speed: {SelectedCreature.stats.speed}";

                MessageBox.Show(statsMessage, "GroundCrasher Selected");

                string name = SelectedCreature.name;

                Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

                _Manager.AddPlayerCreatures(selected);
                _Manager.PrintActors();
            }
            else
            {
                MessageBox.Show("No creature selected!");
            }
        }

    }
}
