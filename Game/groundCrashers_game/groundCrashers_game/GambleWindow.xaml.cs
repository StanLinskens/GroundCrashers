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

        // Define your creature types and elements
        private readonly List<Primaries> creatureTypes = new List<Primaries>
        {
            Primaries.Verdant,
            Primaries.Primal,
            Primaries.Apex,
            Primaries.Sapient,
            Primaries.Synthetic
        };

        private readonly List<Elements> creatureTitanElements = new List<Elements>
        {
            Elements.Chaos,
            Elements.Cosmic,
            Elements.Void,
            Elements.Astral
        };

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

            _mapWindow = mapWindom;
            _gameManager = new Manager();
            _esp32Manager = new Esp32Manager();

            // Set default selections
            TypeComboBox.SelectedIndex = 0;
            ElementComboBox.SelectedIndex = 0;
            creatureId = 0; // Reset creature ID just in case
            playerCoins = ActiveAccount.Active_coins; // Load coins from the active account

            if (_gameManager.getFromJson) _gameManager.LoadAllCreatures();
            else _gameManager.LoadAllCreaturesFromWebAsync();

            UpdateCoinsDisplay();
        }

        private void UpdateCoinsDisplay()
        {
            CoinsDisplay.Text = playerCoins.ToString();
        }

        private bool HasEnoughCoins(int cost)
        {
            playerCoins = ActiveAccount.Active_coins; // Ensure we have the latest coin count
            if (playerCoins >= cost)
            {
                return true;
            }
            else
            {
                creatureId = 0; // Reset creature ID if not enough coins, just in case
                ResultText.Text = "❌ Not enough coins!";
                CreatureResult.Text = "";
                ShowButtons();
                return false;
            }
        }

        private void SpendCoins(int amount)
        {
            playerCoins -= amount;
            ActiveAccount.Active_coins = playerCoins;
            AccountManager.UpdateActiveAccount();
            UpdateCoinsDisplay();
        }

        private string GetRandomCreature()
        {
            creatureId = 0; // Reset creature ID just in case
            int RandomNumber = random.Next(1, 1001);

            // Generate random type and element
            Primaries randomType;
            Elements randomElement;

            if (RandomNumber < 949)
            {
                // Generate random type and element
                randomType = creatureTypes[random.Next(creatureTypes.Count)];
                randomElement = creatureElements[random.Next(creatureElements.Count)];
            }
            else if (RandomNumber < 999 )
            {
                randomType = Primaries.Titan; // Titan is a special case
                randomElement = creatureTitanElements[random.Next(creatureTitanElements.Count)];
            }
            else
            {
                randomType = Primaries.God;
                randomElement = Elements.ALL;
            }


            Creature creature = _gameManager.GetRandomGambleCreature(randomType, randomElement);

            creatureId = creature.id;

            return $" {randomType} - {randomElement}: {creature.name}";
        }

        private string GetTypeSpecificCreature(Primaries selectedType)
        {
            creatureId = 0; // Reset creature ID just in case

            Elements randomElement = creatureElements[random.Next(creatureElements.Count)];

            Creature creature = _gameManager.GetRandomGambleCreature(selectedType, randomElement);

            creatureId = creature.id;

            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        private string GetElementSpecificCreature(Elements selectedElement)
        {
            creatureId = 0; // Reset creature ID just in case

            Primaries randomType = creatureTypes[random.Next(creatureTypes.Count)];

            Creature creature = _gameManager.GetRandomGambleCreature(randomType, selectedElement);
            
            creatureId = creature.id;

            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        private void BasicGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 50;

            if (!HasEnoughCoins(cost)) return;

            SpendCoins(cost);

            string creature = GetRandomCreature();

            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            ShowButtons();

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void ShowButtons()
        {
            if(CreatureResult.Text == "")
            {
                ActionButtonsPanel.Visibility = Visibility.Hidden;

                BasicGambleBtn.IsEnabled = true;
                TypeGambleBtn.IsEnabled = true;
                ElementGambleBtn.IsEnabled = true;

            }
            else
            {
                ActionButtonsPanel.Visibility = Visibility.Visible;

                BasicGambleBtn.IsEnabled = false;
                TypeGambleBtn.IsEnabled = false;
                ElementGambleBtn.IsEnabled = false;
            }
        }

        private void TypeGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 10;

            if (!HasEnoughCoins(cost)) return;

            if (TypeComboBox.SelectedItem == null)
            {
                ResultText.Text = "⚠️ Please select a creature type first!";
                CreatureResult.Text = "";
                return;
            }

            SpendCoins(cost);

            string selectedText = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
            Primaries selectedType = (Primaries)Enum.Parse(typeof(Primaries), selectedText);
            string creature = GetTypeSpecificCreature(selectedType);

            ResultText.Text = "🎉 Congratulations! You received:";
            CreatureResult.Text = creature;

            ShowButtons();

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void ElementGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            creatureId = 0; // Reset creature ID just in case

            const int cost = 20;

            if (!HasEnoughCoins(cost)) return;

            if (ElementComboBox.SelectedItem == null)
            {
                ResultText.Text = "Please select a creature element first!";
                CreatureResult.Text = "";
                ShowButtons();
                return;
            }

            SpendCoins(cost);

            string selectedText = ((ComboBoxItem)ElementComboBox.SelectedItem).Content.ToString();
            Elements selectedElement = (Elements)Enum.Parse(typeof(Elements), selectedText);
            string creature = GetElementSpecificCreature(selectedElement);

            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            ShowButtons();

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void SellCreatureBtn_Click(object sender, RoutedEventArgs e)
        {
            int RandomXp = random.Next(3, 8); // Random coins between 3 and 8
            ResultText.Text = $"you earned {RandomXp} xp";
            CreatureResult.Text = "";
            ShowButtons();

            ActiveAccount.Active_XP += RandomXp; // Add XP to the active account
            AccountManager.LevelUp();
            AccountManager.UpdateActiveAccount();
            _mapWindow.AccountLVLVisualChage();
            creatureId = 0;
        }

        private async void KeepCreatureBtn_Click(object sender, RoutedEventArgs e)
        {
            if(ActiveAccount.Active_Admin)
            {
                AccountManager.AddCreature(creatureId);
            }
            // dont do a else here, just do your wizard struff to add them to card. i want admins to also be able to keep them on cards and off

            if (creatureId != 0)
            {
                try
                {
                    // Show loading message
                    MessageBox.Show($"Writing creature #{creatureId} to card...");

                    // Await the write operation
                    string result = await _esp32Manager.WriteCardIDAsync(creatureId);

                    // Log the exact response for debugging
                    MessageBox.Show($"ESP32 Response: '{result}'");

                    // Check if write was successful
                    if (result.Contains("SUCCESS") || result.Contains("READY_TO_BIND"))
                    {
                        MessageBox.Show($"ESP32 is ready. Now present your NFC card to the reader.");
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

            // reset stuff
            ResultText.Text = $"{creatureId} added to card";
            CreatureResult.Text = "";
            ShowButtons();
            creatureId = 0;
        }
    }
}