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

        /// <summary>
        /// display the GroundCrasher window, where you can select a creature to add to your team.
        /// </summary>
        /// <param name="manager">get the manager that is opened</param>
        /// <param name="gameWindow">get the gamewindow that is opened</param>
        /// <param name="esp32Manager">get the esp that is opened</param>
        public GroundCrasherWindow(Manager manager, GameWindow gameWindow, Esp32Manager esp32Manager)
        {
            InitializeComponent();
            _Manager = manager;
            _gameWindow = gameWindow;
            _esp32Manager = esp32Manager;
        }

        /// <summary>
        /// load the GroundCrasher window, and load the creatures from the json file or from the card.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // if not story mode, load all creatures from the json file
            if (!_Manager.StoryMode)
            {
                LoadCreatureButtons();
            }
            // if story mode, load creatures from the card, or from the account (only if admin)
            else
            {
                // if the account is admin, load creatures from the account
                if (ActiveAccount.Active_Admin)
                {
                    LoadcreatureFromAcount();
                }
                else
                {
                    LoadCreatureButtonsFormCard(allowedIdsFromCard);
                }

            }


        }

        /// <summary>
        /// load the creatures from the account, if the account is admin.
        /// </summary>
        private void LoadcreatureFromAcount()
        {
            // dubble check if the account is admin, if not, show a messagebox
            if (ActiveAccount.Active_Admin)
            {
                // if the account is admin, load the creatures from the account
                LoadCreatureButtonsFormCard(ActiveAccount.Active_Creature_ids);
            }
            else
            {
                MessageBox.Show("You are not an admin, you cannot load creatures from account!");
            }
        }

        /// <summary>
        /// button to scan the card for creatures, and load them into the GroundCrasher window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ScanCardButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var idsFromCard = await _esp32Manager.GetCreatureIDsAsync(); // Get IDs

                // if it is more than 0 than it can load otherwise it will not load and show a messagebox
                if (idsFromCard.Count > 0)
                {
                    // get the ids from the card, and store them in the class-level field
                    allowedIdsFromCard = idsFromCard; // 🟢 store them in the class-level field
                    LoadCreatureButtonsFormCard(allowedIdsFromCard); // 🟢 load them all
                    // show what is loaded
                    _Manager.logs.Add($"Scanned card with Creature IDs: {string.Join(", ", idsFromCard)}");
                }
                else
                {
                    // errror box
                    MessageBox.Show("No creatures found on card.");
                    _Manager.logs.Add("Card scan returned no creatures.");
                }
                // logbox refresh
                _gameWindow.RefreshLogBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// if the user clicks on a creature button, it will select the creature and display its stats in the GroundCrasher window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SelectPokemon_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string name = clicked.Content.ToString();

            Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

            // if not null
            if (selected != null)
            {
                // set the textboxes to the selected creature's stats
                GroundCrasherName.Text = selected.name;
                GroundCrasherType.Text = selected.primary_type.ToString();
                GroundCrasherElement.Text = selected.element.ToString();
                GroundCrasherHP.Text = selected.stats.hp.ToString();
                GroundCrasherAttack.Text = selected.stats.attack.ToString();
                GroundCrasherDefense.Text = selected.stats.defense.ToString();
                GroundCrasherSpeed.Text = selected.stats.speed.ToString();
                try {
                    // loaod creature image from the pack://application:,,,/images/GroundCrasherSprites/{selected.name}.png
                    creatureImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{selected.name}.png", UriKind.Absolute));
                    } 
                catch
                {
                    // if error, load the question mark image
                    creatureImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }

                // create a new creature object with the selected creature's stats
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

        /// <summary>
        /// load the creature buttons from the json file, and display them in the GroundCrasher window.
        /// </summary>
        private async void LoadCreatureButtons()
        {
            // get from the file
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            string json;

            // if file is found, load it from the json file
            if (File.Exists(jsonPath))
            {
                json = File.ReadAllText(jsonPath);
            }
            // if file is not found, load it from the internet
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
                    // if error, show a messagebox
                    MessageBox.Show($"Failed to load creature data from the internet: {ex.Message}");
                    return;
                }
            }

            // load the creatures from the json file
            loadedCreatures = JsonConvert.DeserializeObject<List<Creature>>(json);

            // imit the number of creatures to 225 no titans and god
            foreach (Creature creature in loadedCreatures.Take(225))
            {
                // make new button
                Button btn = new Button
                {
                    Content = creature.name,
                    Margin = new Thickness(5),
                    Height = 40,
                    Style = (Style)FindResource("DarkButton"),
                };

                // add the click event to the button
                btn.Click += SelectPokemon_Click;
                CreatureButtonPanel.Children.Add(btn);
            }
        }

        /// <summary>
        /// load the creature buttons from the card, if the account is admin, or from the json file.
        /// </summary>
        /// <param name="allowedCreatureIds">is the creatures that can be used</param>
        private async void LoadCreatureButtonsFormCard(List<int> allowedCreatureIds)
        {
            // get the locatino
            string jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            jsonPath = System.IO.Path.GetFullPath(jsonPath);

            string json;

            // if the file is found loaded, load it from the json
            if (File.Exists(jsonPath))
            {
                json = File.ReadAllText(jsonPath);
            }
            // if the file is not found, load it from the internet
            else
            {
                try
                {
                    // get it of the internet
                    using (HttpClient client = new HttpClient())
                    {
                        json = await client.GetStringAsync("https://stan.1pc.nl/GroundCrashers/data/creatures.json");
                    }
                }
                catch (Exception ex)
                {
                    // five a messagebox that the file is not found
                    MessageBox.Show($"Failed to load creature data from the internet: {ex.Message}");
                    return;
                }
            }

            // get all creatures from the json file, and filter them by the allowedCreatureIds
            var allCreatures = JsonConvert.DeserializeObject<List<Creature>>(json);
            loadedCreatures = allCreatures.Where(c => allowedCreatureIds.Contains(c.id)).ToList();

            // clear the button panel
            CreatureButtonPanel.Children.Clear();

            // add the buttons for each creature that is allowed    
            foreach (Creature creature in loadedCreatures)
            {
                Button btn = new Button
                {
                    Content = creature.name,
                    Margin = new Thickness(5),
                    Height = 40,
                    Style = (Style)FindResource("DarkButton"),
                };

                // add the click event to the button
                btn.Click += SelectPokemon_Click;
                CreatureButtonPanel.Children.Add(btn);
            }
        }

        /// <summary>
        /// confirm the selected creature and add it to the player's team.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmGroundCrasher_Click(object sender, RoutedEventArgs e)
        {
            // if not null, add the selected creature to the player's team
            if (SelectedCreature != null)
            {
                string name = SelectedCreature.name;

                // find it in the loadedCreatures list
                Creature selected = loadedCreatures.FirstOrDefault(c => c.name == name);

                //add it to the player's team
                _Manager.AddPlayerCreatures(selected);

                // Tell GameWindow to update the logbox
                _gameWindow.RefreshLogBox();
            }
            else
            {
                // if no creature is selected, show a messagebox
                _Manager.logs.Add("no creature selected added to team");
                _gameWindow.RefreshLogBox();
            }
        }
    }
}