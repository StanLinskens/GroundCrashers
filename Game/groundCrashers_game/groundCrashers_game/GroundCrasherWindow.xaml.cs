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
using System.Net.Http;
using System.Diagnostics;


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

        private Esp32Manager _esp32Manager;

        List<int> allowedIdsFromCard = new List<int>{};

        List<int> WriteToCard = new List<int>();

        public GroundCrasherWindow(Manager manager, GameWindow gameWindow, Esp32Manager esp32Manager)
        {
            InitializeComponent();
            _Manager = manager;
            _gameWindow = gameWindow;
            _esp32Manager = esp32Manager;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_Manager.StoryMode)
            {
                LoadCreatureButtons();
            }
            else
            {
                LoadCreatureButtonsFormCard(allowedIdsFromCard);
            }


        }
        
        private async void ScanCardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var idsFromCard = await _esp32Manager.GetCreatureIDsAsync(); // Get 13 IDs or more!

                if (idsFromCard.Count > 0)
                {
                    allowedIdsFromCard = idsFromCard; // 🟢 store them in the class-level field
                    LoadCreatureButtonsFormCard(allowedIdsFromCard); // 🟢 load them all
                    _Manager.logs.Add($"Scanned card with Creature IDs: {string.Join(", ", idsFromCard)}");
                }
                else
                {
                    MessageBox.Show("No creatures found on card.");
                    _Manager.logs.Add("Card scan returned no creatures.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

        private async void LoadCreatureButtons()
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            string json;

            if (File.Exists(jsonPath))
            {
                json = File.ReadAllText(jsonPath);
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        json = await client.GetStringAsync("https://stan.1pc.nl/GroundCrashers/data/creatures.json");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load creature data from the internet: {ex.Message}");
                    return;
                }
            }

            loadedCreatures = JsonConvert.DeserializeObject<List<Creature>>(json);

            foreach (Creature creature in loadedCreatures.Take(225))
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


        private async void LoadCreatureButtonsFormCard(List<int> allowedCreatureIds)
        {
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            string json;

            if (File.Exists(jsonPath))
            {
                json = File.ReadAllText(jsonPath);
            }
            else
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        json = await client.GetStringAsync("https://stan.1pc.nl/GroundCrashers/data/creatures.json");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to load creature data from the internet: {ex.Message}");
                    return;
                }
            }

            var allCreatures = JsonConvert.DeserializeObject<List<Creature>>(json);
            loadedCreatures = allCreatures.Where(c => allowedCreatureIds.Contains(c.id)).ToList();

            CreatureButtonPanel.Children.Clear();

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

        private void AddtoCard_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCreature != null)
            {
                string name = SelectedCreature.name;

                //espConnectionWindow..AddCreatureToCard(name);
            }
            else
            {
                _Manager.logs.Add("no creature selected to add to card!");
            }
        }

        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCreature != null)
            {
                if (!WriteToCard.Contains(SelectedCreature.id))
                {
                    WriteToCard.Add(SelectedCreature.id);
                    Debug.WriteLine($"✅ Added {SelectedCreature.name} (ID: {SelectedCreature.id}) to WriteToCard list.");
                }
                else
                {
                    Debug.WriteLine($"⚠️ {SelectedCreature.name} is already in the WriteToCard list.");
                }
            }
            else
            {
                Debug.WriteLine("❌ No creature selected. Cannot add to list.");
            }
        }

    }
}
