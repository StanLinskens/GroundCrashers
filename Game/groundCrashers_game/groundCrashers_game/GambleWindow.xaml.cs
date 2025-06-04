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
        private Random random = new Random();
        Manager gameManager;

        // Define your creature types and elements
        private readonly List<Primaries> creatureTypes = new List<Primaries>
        {
            Primaries.Verdant,
            Primaries.Primal,
            Primaries.Apex,
            Primaries.Sapient,
            Primaries.Synthetic
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

        public GambleWindow()
        {
            InitializeComponent();

            gameManager = new Manager();

            // Set default selections
            TypeComboBox.SelectedIndex = 0;
            ElementComboBox.SelectedIndex = 0;
            playerCoins = ActiveAccount.Active_coins; // Load coins from the active account

            gameManager.LoadAllCreaturesFromWebAsync();

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
                ResultText.Text = "❌ Not enough coins!";
                CreatureResult.Text = "";
                return false;
            }
        }

        private void SpendCoins(int amount)
        {
            playerCoins -= amount;
            ActiveAccount.Active_coins = playerCoins;
            UpdateCoinsDisplay();
        }

        private string GetRandomCreature()
        {
            // Generate random type and element
            Primaries randomType = creatureTypes[random.Next(creatureTypes.Count)];
            Elements randomElement = creatureElements[random.Next(creatureElements.Count)];

            Creature creature = gameManager.GetRandomGambleCreature(randomType, randomElement);

            return $" {randomType} - {randomElement}: {creature.name}";
        }

        private string GetTypeSpecificCreature(Primaries selectedType)
        {
            Elements randomElement = creatureElements[random.Next(creatureElements.Count)];

            Creature creature = gameManager.GetRandomGambleCreature(selectedType, randomElement);

            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        private string GetElementSpecificCreature(Elements selectedElement)
        {
            Primaries randomType = creatureTypes[random.Next(creatureTypes.Count)];

            Creature creature = gameManager.GetRandomGambleCreature(randomType, selectedElement);

            return $"{creature.primary_type} - {creature.element}: {creature.name}";
        }

        private void BasicGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            const int cost = 5;

            if (!HasEnoughCoins(cost)) return;

            SpendCoins(cost);

            string creature = GetRandomCreature();

            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void ShowButtons()
        {

        }

        private void TypeGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            const int cost = 15;

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

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void ElementGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            const int cost = 30;

            if (!HasEnoughCoins(cost)) return;

            if (ElementComboBox.SelectedItem == null)
            {
                ResultText.Text = "Please select a creature element first!";
                CreatureResult.Text = "";
                return;
            }

            SpendCoins(cost);

            string selectedText = ((ComboBoxItem)ElementComboBox.SelectedItem).Content.ToString();
            Elements selectedElement = (Elements)Enum.Parse(typeof(Elements), selectedText);
            string creature = GetElementSpecificCreature(selectedElement);

            ResultText.Text = "You received:";
            CreatureResult.Text = creature;

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        private void SellBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}