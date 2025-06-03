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

        // Define your creature types and elements
        private readonly List<string> creatureTypes = new List<string>
        {
            "Verdant", "Primal", "Apex", "Sapient", "Synthetic"
        };

        private readonly List<string> creatureElements = new List<string>
        {
            "Nature", "Ice", "Toxic", "Fire", "Water", "Draconic", "Earth",
            "Dark", "Wind", "Psychic", "Light", "GOD", "Demonic", "Electric",
            "Acid", "Magnetic"
        };

        // Sample creature names for each type (you can expand this)
        private readonly Dictionary<string, List<string>> creaturesByType = new Dictionary<string, List<string>>
        {
            { "Verdant", new List<string> { "Forest Guardian", "Leaf Dancer", "Moss Walker", "Tree Spirit" } },
            { "Primal", new List<string> { "Beast King", "Wild Runner", "Savage Howler", "Feral Hunter" } },
            { "Apex", new List<string> { "Alpha Predator", "Dominant Force", "Peak Hunter", "Supreme Beast" } },
            { "Sapient", new List<string> { "Wise Scholar", "Mind Reader", "Ancient Sage", "Thoughtful One" } },
            { "Synthetic", new List<string> { "Cyber Beast", "Metal Guardian", "Digital Spirit", "Tech Warrior" } }
        };

        // Sample creature names for each element (you can expand this)
        private readonly Dictionary<string, List<string>> creaturesByElement = new Dictionary<string, List<string>>
        {
            { "Nature", new List<string> { "Bloom Sprite", "Vine Whip", "Petal Storm", "Root Crawler" } },
            { "Ice", new List<string> { "Frost Bite", "Crystal Shard", "Snow Drift", "Frozen Heart" } },
            { "Toxic", new List<string> { "Poison Fang", "Venom Drop", "Toxic Cloud", "Acid Spit" } },
            { "Fire", new List<string> { "Flame Dancer", "Ember Storm", "Blaze Runner", "Inferno Beast" } },
            { "Water", new List<string> { "Wave Rider", "Splash Guardian", "Tide Turner", "Aqua Spirit" } },
            { "Draconic", new List<string> { "Dragon Heir", "Scale Bearer", "Wing Fury", "Breath Wielder" } },
            { "Earth", new List<string> { "Stone Crusher", "Boulder Fist", "Rock Slide", "Mountain Spirit" } },
            { "Dark", new List<string> { "Shadow Walker", "Night Stalker", "Void Touched", "Eclipse Beast" } },
            { "Wind", new List<string> { "Gale Force", "Whirlwind", "Storm Caller", "Air Dancer" } },
            { "Psychic", new List<string> { "Mind Bender", "Thought Reader", "Psychic Wave", "Mental Force" } },
            { "Light", new List<string> { "Radiant Being", "Sun Blessed", "Holy Guardian", "Light Bringer" } },
            { "GOD", new List<string> { "Divine Avatar", "Celestial Warrior", "Sacred Beast", "Holy Champion" } },
            { "Demonic", new List<string> { "Hell Hound", "Demon Spawn", "Cursed One", "Infernal Beast" } },
            { "Electric", new List<string> { "Thunder Strike", "Lightning Bolt", "Spark Dancer", "Volt Runner" } },
            { "Acid", new List<string> { "Corrosive Touch", "Acid Rain", "Melting Force", "Caustic Beast" } },
            { "Magnetic", new List<string> { "Iron Puller", "Magnet Force", "Metal Bender", "Pole Shifter" } }
        };

        public GambleWindow()
        {
            InitializeComponent();
            UpdateCoinsDisplay();

            // Set default selections
            TypeComboBox.SelectedIndex = 0;
            ElementComboBox.SelectedIndex = 0;
        }

        private void UpdateCoinsDisplay()
        {
            CoinsDisplay.Text = playerCoins.ToString();
        }

        private bool HasEnoughCoins(int cost)
        {
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
            UpdateCoinsDisplay();
        }

        private string GetRandomCreature()
        {
            // Generate random type and element
            string randomType = creatureTypes[random.Next(creatureTypes.Count)];
            string randomElement = creatureElements[random.Next(creatureElements.Count)];

            // Get random creature name
            string creatureName = GetRandomCreatureName();

            return $"{randomElement} {randomType} - {creatureName}";
        }

        private string GetRandomCreatureName()
        {
            // Simple creature name generator
            string[] prefixes = { "Mighty", "Swift", "Ancient", "Brave", "Fierce", "Noble", "Wild", "Mystic" };
            string[] suffixes = { "Claw", "Fang", "Wing", "Horn", "Tail", "Eye", "Heart", "Soul" };

            return $"{prefixes[random.Next(prefixes.Length)]}{suffixes[random.Next(suffixes.Length)]}";
        }

        private string GetTypeSpecificCreature(string selectedType)
        {
            string randomElement = creatureElements[random.Next(creatureElements.Count)];
            string creatureName;

            if (creaturesByType.ContainsKey(selectedType))
            {
                var typeCreatures = creaturesByType[selectedType];
                creatureName = typeCreatures[random.Next(typeCreatures.Count)];
            }
            else
            {
                creatureName = GetRandomCreatureName();
            }

            return $"{randomElement} {selectedType} - {creatureName}";
        }

        private string GetElementSpecificCreature(string selectedElement)
        {
            string randomType = creatureTypes[random.Next(creatureTypes.Count)];
            string creatureName;

            if (creaturesByElement.ContainsKey(selectedElement))
            {
                var elementCreatures = creaturesByElement[selectedElement];
                creatureName = elementCreatures[random.Next(elementCreatures.Count)];
            }
            else
            {
                creatureName = GetRandomCreatureName();
            }

            return $"{selectedElement} {randomType} - {creatureName}";
        }

        private void BasicGambleBtn_Click(object sender, RoutedEventArgs e)
        {
            const int cost = 5;

            if (!HasEnoughCoins(cost)) return;

            SpendCoins(cost);

            string creature = GetRandomCreature();

            ResultText.Text = "🎉 Congratulations! You received:";
            CreatureResult.Text = creature;

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
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

            string selectedType = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
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
                ResultText.Text = "⚠️ Please select a creature element first!";
                CreatureResult.Text = "";
                return;
            }

            SpendCoins(cost);

            string selectedElement = ((ComboBoxItem)ElementComboBox.SelectedItem).Content.ToString();
            string creature = GetElementSpecificCreature(selectedElement);

            ResultText.Text = "🎉 Congratulations! You received:";
            CreatureResult.Text = creature;

            // Here you would add the creature to the player's collection
            // AddCreatureToCollection(creature);
        }

        // Method to connect to your main game's coin system
        public void SetPlayerCoins(int coins)
        {
            playerCoins = coins;
            UpdateCoinsDisplay();
        }

        // Method to get current coins (for saving back to main game)
        public int GetPlayerCoins()
        {
            return playerCoins;
        }
    }
}