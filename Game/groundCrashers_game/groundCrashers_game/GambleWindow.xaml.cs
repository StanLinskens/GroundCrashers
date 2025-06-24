using DocumentFormat.OpenXml.Drawing.Charts;
using groundCrashers_game.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace groundCrashers_game
{
    public partial class GambleWindow : Window
    {
        private int playerCoins = 100; // Starting coins - you can modify this or connect to your game's coin system
        private int creatureId = 0;
        private Random random = new Random();
        Manager _gameManager;
        private LevelMapWindow _mapWindow;
        private Esp32Manager _esp32Manager;

        // the creature types
        private readonly List<Primaries> creatureTypes = new List<Primaries>
        {
            Primaries.Verdant,
            Primaries.Primal,
            Primaries.Apex,
            Primaries.Sapient,
            Primaries.Synthetic
        };

        // the creatures elements for titan
        private readonly List<Elements> creatureTitanElements = new List<Elements>
        {
            Elements.Chaos,
            Elements.Cosmic,
            Elements.Void,
            Elements.Astral
        };

        // the creature elements
        private readonly List<Elements> creatureElements = new List<Elements>
        {
            Elements.Nature, 
            Elements.Ice, 
            Elements.Toxic, 
            Elements.Fire, 
            Elements.Water, 
            Elements.Draconic, 
            Elements.Earth,
            Elements.Dark, 
            Elements.Wind, 
            Elements.Psychic, 
            Elements.Light, 
            Elements.Demonic, 
            Elements.Electric,
            Elements.Acid, 
            Elements.Magnetic
        };

        public GambleWindow(LevelMapWindow mapWindom)
        {
            InitializeComponent();

            // get the map and set a new manager and esp
            _mapWindow = mapWindom;
            _gameManager = new Manager();
            _esp32Manager = new Esp32Manager();

            // Set default selections
            TypeComboBox.SelectedIndex = 0;
            ElementComboBox.SelectedIndex = 0;
            creatureId = 0; // Reset creature ID just in case
            playerCoins = ActiveAccount.Active_coins; // Load coins from the active account

            // load the creatures from json if it is true
            if (_gameManager.getFromJson) _gameManager.LoadAllCreatures();
            // else load it from web
            else _gameManager.LoadAllCreaturesFromWebAsync();

            // update the coins display
            UpdateCoinsDisplay();
        }

        /// <summary>
        /// coins display update
        /// </summary>
        private void UpdateCoinsDisplay()
        {
            CoinsDisplay.Text = playerCoins.ToString();
        }

        /// <summary>
        /// check if the player has enough coins
        /// </summary>
        /// <param name="cost">the amount of coins needed</param>
        /// <returns>returns true or false (does have enough coins or not)</returns>
        private bool HasEnoughCoins(int cost)
        {
            // update player coins just to be shure
            playerCoins = ActiveAccount.Active_coins; // Ensure we have the latest coin count
            // if player has enough coins return true
            if (playerCoins >= cost)
            {
                return true;
            }
            // if not enough coins
            else
            {
                creatureId = 0; // Reset creature ID if not enough coins, just in case
                ResultText.Text = "❌ Not enough coins!"; // textblock change
                CreatureResult.Text = "";
                ShowButtons();
                return false; // return false
            }
        }

        /// <summary>
        /// spend the coins from the account
        /// </summary>
        /// <param name="amount">amount is the coins needed to buy it</param>
        private void SpendCoins(int amount)
        {
            playerCoins -= amount; 
            ActiveAccount.Active_coins = playerCoins; // account coins is the player coins
            AccountManager.UpdateActiveAccount(); // update the active acount
            UpdateCoinsDisplay(); // update teh coins display
        }

        /// <summary>
        /// get a random creature from all the creatures
        /// </summary>
        /// <returns>returns the random creature</returns>
        private string GetRandomCreature()
        {
            creatureId = 0; // Reset creature ID just in case
            int RandomNumber = random.Next(1, 1001);

            // type and element
            Primaries randomType;
            Elements randomElement;

            // 94.9% chance on normal creature
            if (RandomNumber < 949)
            {
                // Generate random type and element
                randomType = creatureTypes[random.Next(creatureTypes.Count)];
                randomElement = creatureElements[random.Next(creatureElements.Count)];
            }
            // 5% chance on titan type creature
            else if (RandomNumber < 999 )
            {
                randomType = Primaries.Titan; // Titan is a special case
                randomElement = creatureTitanElements[random.Next(creatureTitanElements.Count)]; // random titan element
            }
            // 0.1% chance to get god
            else
            {
                
                randomType = Primaries.God;
                randomElement = Elements.ALL;
            }

            // creature is the creaturer earned from the gamble
            Creature creature = _gameManager.GetRandomGambleCreature(randomType, randomElement);

            // creature id is the creature id
            creatureId = creature.id;

            // returns the text items
            return $" {randomType} - {randomElement}: {creature.name}";
        }

        /// <summary>
        /// get a type specific creature
        /// </summary>
        /// <param name="selectedType">the selected type</param>
        /// <returns>returns a creature of that type</returns>
        private string GetTypeSpecificCreature(Primaries selectedType)
        {
            creatureId = 0; // Reset creature ID just in case

            // get random element
            Elements randomElement = creatureElements[random.Next(creatureElements.Count)];

            // get random creature
            Creature creature = _gameManager.GetRandomGambleCreature(selectedType, randomElement);

            creatureId = creature.id;

            // return info
            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        /// <summary>
        /// get element specific creature
        /// </summary>
        /// <param name="selectedElement">the selected element</param>
        /// <returns>returns a creature of that element</returns>
        private string GetElementSpecificCreature(Elements selectedElement)
        {
            creatureId = 0; // Reset creature ID just in case

            // random primary type
            Primaries randomType = creatureTypes[random.Next(creatureTypes.Count)];

            // get random creature
            Creature creature = _gameManager.GetRandomGambleCreature(randomType, selectedElement);
            
            creatureId = creature.id;

            // return the creature
            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        /// <summary>
        /// get a random creature with no selected type or element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasicGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 50; // the amount it cost

            // check if player has enough money
            if (!HasEnoughCoins(cost)) return;

            // spend the money
            SpendCoins(cost);

            // return the info
            string creature = GetRandomCreature();

            // set the text
            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            // show the buttons
            ShowButtons();
        }

        /// <summary>
        /// show the buttons for selling/keeping the creature
        /// </summary>
        private void ShowButtons()
        {
            // if the creature result rext is empty hide sell/keep buttons but activate the buy buttons
            if(CreatureResult.Text == "")
            {
                ActionButtonsPanel.Visibility = Visibility.Hidden;

                BasicGambleBtn.IsEnabled = true;
                TypeGambleBtn.IsEnabled = true;
                ElementGambleBtn.IsEnabled = true;

            }
            // disable the buy buttons but activate the sell/keep buttons
            else
            {
                ActionButtonsPanel.Visibility = Visibility.Visible;

                BasicGambleBtn.IsEnabled = false;
                TypeGambleBtn.IsEnabled = false;
                ElementGambleBtn.IsEnabled = false;
            }
        }

        /// <summary>
        /// get a type specific creature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TypeGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 10; // amount of money it cost

            // check if it has enough money
            if (!HasEnoughCoins(cost)) return;

            // if it is null
            if (TypeComboBox.SelectedItem == null)
            {
                ResultText.Text = "⚠️ Please select a creature type first!";
                CreatureResult.Text = "";
                return;
            }

            // spend the money
            SpendCoins(cost);

            // get the info and put it in the veriable
            string selectedText = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
            Primaries selectedType = (Primaries)Enum.Parse(typeof(Primaries), selectedText);
            // get the creature
            string creature = GetTypeSpecificCreature(selectedType);

            // set the text
            ResultText.Text = "🎉 Congratulations! You received:";
            CreatureResult.Text = creature;

            // show the buttons
            ShowButtons();
        }

        /// <summary>
        /// element specific creature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 20; // amount it cost

            // check if enough money
            if (!HasEnoughCoins(cost)) return;

            // check if null
            if (ElementComboBox.SelectedItem == null)
            {
                ResultText.Text = "Please select a creature element first!";
                CreatureResult.Text = "";
                ShowButtons();
                return;
            }

            // spend coins
            SpendCoins(cost);

            // get the elemnt
            string selectedText = ((ComboBoxItem)ElementComboBox.SelectedItem).Content.ToString();
            Elements selectedElement = (Elements)Enum.Parse(typeof(Elements), selectedText);
            // get the creature
            string creature = GetElementSpecificCreature(selectedElement);

            // set the text
            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            // show the buttons
            ShowButtons();
        }

        /// <summary>
        /// if the player wants to sell for xp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SellCreatureBtn_Click(object sender, RoutedEventArgs e)
        {
            int RandomXp = random.Next(3, 8); // Random coins between 3 and 8
            ResultText.Text = $"you earned {RandomXp} xp";
            CreatureResult.Text = "";
            // show other buttons
            ShowButtons();

            // add the xp and update the account
            ActiveAccount.Active_XP += RandomXp; // Add XP to the active account
            AccountManager.LevelUp();
            AccountManager.UpdateActiveAccount();
            // change the xp amount on the account
            _mapWindow.AccountLVLVisualChage();
            creatureId = 0;// reset the creature id
        }

        /// <summary>
        /// if the player wants to keep the creature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void KeepCreatureBtn_Click(object sender, RoutedEventArgs e)
        {
            // to add the creature to the account
            string message = AccountManager.AddCreature(creatureId);

            // if error messages, show them and return
            if (message == "not a creature" || message == "Creature is already owned.") { MessageBox.Show(message); return; }
            // dont do a else here, just do your wizard struff to add them to card. i want admins to also be able to keep them on cards and off

            // if the creatureId is not 0 and the message is success, write it to the card
            if (creatureId != 0 && message == "Creature added successfully!")
            {
                try
                {
                    // Await the write operation
                    string result = await _esp32Manager.WriteCardIDAsync(creatureId);


                    // Check if write was successful
                    if (result.Contains("SUCCESS") || result.Contains("READY_TO_BIND"))
                    {

                        // reset stuff
                        ResultText.Text = $"{creatureId} added to card";
                        CreatureResult.Text = "";
                        ShowButtons();
                        creatureId = 0;
                    }
                    else if (result.StartsWith("Error"))
                    {
                        MessageBox.Show($"Connection error: {result}");
                    }
                    else
                    {
                        MessageBox.Show($"Unexpected response: {result}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error writing to card: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No creature selected to add to card!");
            }
        }
    }
}