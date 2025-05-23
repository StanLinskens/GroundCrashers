﻿using groundCrashers_game.classes;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using static groundCrashers_game.classes.Manager;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        Manager gameManager;
        private Button fightButton;
        private Button bagButton;
        private Button groundCrashersButton;
        private Button runButton;
        private WrapPanel actionButtonsPanel;

        private Dictionary<Elements, string> CreatureElementColor = new()
        {
            { Elements.Nature, "#9BCF53" },
            { Elements.Ice, "#B2EBF2" },
            { Elements.Toxic, "#CE93D8" },
            { Elements.Fire, "#f0563b" },
            { Elements.Water, "#81D4FA" },
            { Elements.Draconic, "#cd3737" },
            { Elements.Earth, "#D2B48C" },
            { Elements.Dark, "#444444" },
            { Elements.Wind, "#86dcd3" },
            { Elements.Psychic, "#F48FB1" },
            { Elements.Light, "#fff8ba" },
            { Elements.Demonic, "#b92222" },
            { Elements.Electric, "#f6e15b" },
            { Elements.Acid, "#aae666" },
            { Elements.Magnetic, "#c0c0c0" }
        };

        private Dictionary<Primaries, string> CreaturePrimaryColor = new()
        {
            { Primaries.Verdant, "#228B22" },
            { Primaries.Primal, "#FFC300" },
            { Primaries.Apex, "#D7263D" },
            { Primaries.Sapient, "#800080" },
            { Primaries.Synthetic, "#0066CC" },
        };

        public GameWindow()
        {
            InitializeComponent();

            // Initialize the game manager
            gameManager = new Manager();
            //gameManager.LoadAllCreatures();
            gameManager.LoadGameData();
            gameManager.LoadActorsForBattleMode();

            UpdateBattleUI();

            // Find the WrapPanel in the XAML layout
            FindActionButtonsPanel();

            RandomScenarioGenerator();
        }

        public void RefreshLogBox()
        {
            gameManager.ControlLogs();
            logbox.Text = string.Join("\n", gameManager.logs);
        }

        public void UpdateBattleUI()
        {
            // Instead of grabbing player.Creatures[0], we use ActivePlayerCreature
            var playerCreature = gameManager.ActivePlayerCreature;
            if (playerCreature != null)
            {
                PlayerCreatureName.Text = playerCreature.name;
                PlayerHealthText.Text = playerCreature.stats.hp.ToString() + "/" + playerCreature.stats.max_hp.ToString();
                PlayerHealthBar.Value = playerCreature.stats.hp;
                PlayerHealthBar.Maximum = playerCreature.stats.max_hp;
                PlayerCreatureName.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        CreaturePrimaryColor.GetValueOrDefault(
                            playerCreature.primary_type, 
                            "#555555")));
                PlayerCreatureBorder.BorderBrush =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreatureElementColor.GetValueOrDefault(
                            playerCreature.element,
                            "#555555")));

                PlayerEllipse.Fill =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreatureElementColor.GetValueOrDefault(
                            playerCreature.curse,
                            "#222")));
                try
                {
                    PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{playerCreature.name}.png", UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error loading image: {ex.Message}");
                    EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }

            }
            else
            {
                PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));

                string PlayerHealthT = PlayerHealthText.Text.ToString();
                string[] PlayerHealth_split = PlayerHealthT.Split('/');

                PlayerHealthBar.Value = 0;
                PlayerHealthText.Text = "0/" + PlayerHealth_split[1];

            }


            // Likewise for the CPU’s active creature:
            var cpuCreature = gameManager.ActiveCpuCreature;
            if (cpuCreature != null)
            {
                EnemyCreatureName.Text = cpuCreature.name;
                EnemyHealthText.Text = cpuCreature.stats.hp.ToString() + "/" + cpuCreature.stats.max_hp.ToString();
                EnemyHealthBar.Value = cpuCreature.stats.hp;
                EnemyHealthBar.Maximum = cpuCreature.stats.max_hp;

                EnemyCreatureName.Foreground =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreaturePrimaryColor.GetValueOrDefault( 
                            cpuCreature.primary_type,
                            "#555555")));
                EnemyCreatureBorder.BorderBrush =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreatureElementColor.GetValueOrDefault(
                            cpuCreature.element,
                            "#555555")));
                EnemyEllipse.Fill =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreatureElementColor.GetValueOrDefault(
                            cpuCreature.curse,
                                "#222")));
                try
                {
                    EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{cpuCreature.name}.png", UriKind.Absolute));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error loading image: {ex.Message}");
                    EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }
            }
            else
            {
                EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));

                string EnemyHealthT = EnemyHealthText.Text.ToString();
                string[] EnemyHealt_split = EnemyHealthT.Split('/');

                EnemyHealthBar.Value = 0;
                EnemyHealthText.Text = "0/" + EnemyHealt_split[1];
                this.Close();
            }
        }

        // Event handler for when the window is loaded
        private void RandomScenarioGenerator()
        {
            // 1) Get random enum values
            var randomBiome = Manager.GetRandomBiome();
            var randomTime = Manager.GetRandomDaytime();
            var randomWeather = Manager.GetRandomWeather();

            // 2) Update the TextBlocks
            BiomeText.Text = randomBiome.ToString().ToUpper();
            BiomeIcon.Text = GetBiomeEmoji(randomBiome);

            DaytimeText.Text = randomTime.ToString().ToUpper();
            DaytimeIcon.Text = GetDaytimeEmoji(randomTime);

            WeatherText.Text = randomWeather.ToString().ToUpper();
            WeatherIcon.Text = GetWeatherEmoji(randomWeather);

            // 3) Set the background image based on the random biome
            BiomeBackground.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/battleGrounds/{randomBiome.ToString().ToLower()}.jpg", UriKind.Absolute));
            try { 
                  EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{gameManager.ActiveCpuCreature.name}.png", UriKind.Absolute));
                  //PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{gameManager.ActivePlayerCreature.name}.png", UriKind.Absolute));

            } catch
            {
                //MessageBox.Show("Could not find the image for the enemy creature. Please check the image path.");
            }
        }

        // Helper: map each Biome to a simple emoji
        private string GetBiomeEmoji(Biomes b)
        {
            switch (b)
            {
                case Biomes.Forest: return "🌲";
                case Biomes.Desert: return "🏜️";
                case Biomes.Mountain: return "⛰️";
                case Biomes.Highlands: return "🌄";
                case Biomes.Glacier: return "🧊";
                case Biomes.Swamp: return "🦆";
                case Biomes.Ocean: return "🌊";
                case Biomes.Volcano: return "🌋";
                case Biomes.Savanna: return "🦁";
                case Biomes.Jungle: return "🌴";
                case Biomes.Tundra: return "❄️";
                case Biomes.Cave: return "🕯️";
                case Biomes.Ruins: return "🏰";
                case Biomes.Marsh: return "🦢";
                case Biomes.CrystalCavern: return "💎";
                case Biomes.Wasteland: return "🏜️";
                default: return "❓";
            }
        }

        // Helper: map each Daytime to an emoji
        private string GetDaytimeEmoji(Daytimes d)
        {
            switch (d)
            {
                case Daytimes.Dawn: return "🌅";
                case Daytimes.Day: return "☀️";
                case Daytimes.Dusk: return "🌇";
                case Daytimes.Night: return "🌙";
                default: return "❓";
            }
        }

        // Helper: map each Weather to an emoji
        private string GetWeatherEmoji(Weathers w)
        {
            switch (w)
            {
                case Weathers.Sunny: return "☀️";
                case Weathers.Rainy: return "🌧️";
                case Weathers.Cloudy: return "☁️";
                case Weathers.Foggy: return "🌫️";
                case Weathers.Windy: return "🌬️";
                case Weathers.Hail: return "🌨️";
                case Weathers.Sandstorm: return "🏜️";
                case Weathers.Clear: return "🆓";
                default: return "❓";
            }
        }

        private void FindActionButtonsPanel()
        {
            try
            {
                Grid mainGrid = (Grid)((Viewbox)Content).Child;

                // Get the options Border from the last row (Grid.Row="4")
                Border optionsBorder = (Border)mainGrid.Children[mainGrid.RowDefinitions.Count - 1];

                // Get the Grid inside the options Border
                Grid optionsGrid = (Grid)optionsBorder.Child;

                // Now look for the WrapPanel in column 1
                foreach (UIElement child in optionsGrid.Children)
                {
                    if (child is WrapPanel panel && Grid.GetColumn(panel) == 1)
                    {
                        actionButtonsPanel = panel;
                        StoreButtonReferences();
                        break;
                    }
                }

                if (actionButtonsPanel == null)
                {
                    MessageBox.Show("Could not find action buttons panel. UI structure may have changed.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while locating action buttons panel: {ex.Message}");
            }
        }


        private void StoreButtonReferences()
        {
            // Get references to the original buttons for later use
            foreach (UIElement element in actionButtonsPanel.Children)
            {
                if (element is Button button)
                {
                    string content = button.Content.ToString();
                    switch (content)
                    {
                        case "FIGHT":
                            fightButton = button;
                            break;
                        case "SWAP":
                            bagButton = button;
                            break;
                        case "GROUNDCRASHERS":
                            groundCrashersButton = button;
                            break;
                        case "RUN":
                            runButton = button;
                            break;
                    }
                }
            }
        }

        private void Fight_Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameManager.ActivePlayerCreature != null)
            {
                // Replace the current buttons with combat options
                ShowCombatOptions();
            }
            else
            {
                gameManager.logs.Add("You need to select a creature first.");
            }
            RefreshLogBox();
        }

        private void ShowCombatOptions()
        {
            // Save original buttons if not already saved
            if (fightButton == null)
            {
                StoreButtonReferences();
            }

            // Clear the current buttons from the panel
            actionButtonsPanel.Children.Clear();

            // Create new combat option buttons
            Button attackButton = CreateButton("ATTACK", "#591C1C", "#7A2929", Attack_Button_Click);
            Button elementButton = CreateButton("ELEMENT", "#802828", "#7A2929", Element_Button_Click);
            Button defendButton = CreateButton("DEFEND", "#1C3959", "#295D7A", Defend_Button_Click);
            Button backButton = CreateButton("BACK", "Gray", "DarkGray", Back_Button_Click);

            // Add the new buttons to the panel
            actionButtonsPanel.Children.Add(attackButton);
            actionButtonsPanel.Children.Add(elementButton);
            actionButtonsPanel.Children.Add(defendButton);
            actionButtonsPanel.Children.Add(backButton);
        }

        private Button CreateButton(string content, string background, string border, RoutedEventHandler clickHandler)
        {
            Button button = new Button
            {
                Content = content,
                Height = 60,
                Width = 400,
                Margin = new Thickness(10),
                FontSize = 20,
                Background = (Brush)new BrushConverter().ConvertFromString(background),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString(border),
                Style = (Style)FindResource("DarkButton"), // Apply the shared DarkButton style
            };

            // Add DropShadowEffect
            button.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                ShadowDepth = 3,
                BlurRadius = 5,
                Opacity = 0.7
            };

            // Add click event handler
            button.Click += clickHandler;

            return button;
        }

        private void Attack_Button_Click(object sender, RoutedEventArgs e)
        {
            gameManager.ProcessTurn(ActionType.Attack);

            UpdateBattleUI();
            // After attack, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void Element_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement elemental attack logic
            gameManager.ProcessTurn(ActionType.ElementAttack);

            UpdateBattleUI();
            // After attack, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void Defend_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement defense logic
            gameManager.ProcessTurn(ActionType.Defend);

            UpdateBattleUI();
            // After defense, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void RestoreMainActionButtons()
        {
            actionButtonsPanel.Children.Clear();

            if (fightButton != null) actionButtonsPanel.Children.Add(fightButton);
            if (bagButton != null) actionButtonsPanel.Children.Add(bagButton);
            if (groundCrashersButton != null) actionButtonsPanel.Children.Add(groundCrashersButton);
            if (runButton != null) actionButtonsPanel.Children.Add(runButton);
        }


        private void Swap_Button_Click(object sender, RoutedEventArgs e)
        {
            Actor playerActor = gameManager.GetPlayerActor();
            if (playerActor.Creatures.Count < 3)
            {
                gameManager.logs.Add("You need at least 3 creatures to swap.");
            }
            else
            {
                //Replace the current buttons with combat options
                ShowSwapOptions();
            }
            RefreshLogBox();
        }

        private void ShowSwapOptions()
        {
            // Save original buttons if not already saved
            if (fightButton == null)
            {
                StoreButtonReferences();
            }

            // Clear the current buttons from the panel
            actionButtonsPanel.Children.Clear();

            Actor playerActor = gameManager.GetPlayerActor();

            foreach (Creature c in playerActor.Creatures)
            {
                if(c.alive)
                {
                    Button Creature = CreateButton(c.name.ToString() ?? "Not Found", CreaturePrimaryColor.GetValueOrDefault(c.primary_type) ?? "#555555", CreatureElementColor.GetValueOrDefault(c.element) ?? "#555555", Creature_Button_Click);
                    actionButtonsPanel.Children.Add(Creature);
                }
            }
           
            Button back_S_Button = CreateButton("BACK", "Gray", "DarkGray", Back_S_Button_Click);

            // Add the new buttons to the panel

            actionButtonsPanel.Children.Add(back_S_Button);
        }
        private void Creature_Button_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string name = clicked.Content.ToString() ?? "default";

            Actor playerActor = gameManager.GetPlayerActor();

            bool IsAlive = playerActor.Creatures.FirstOrDefault(c => c.name == name)?.alive ?? false;

            if (gameManager.ActivePlayerCreature != null && IsAlive)
            {
                gameManager.CurrentPlayerCreatureSet(name);
                gameManager.ProcessTurn(ActionType.Swap);
                gameManager.logs.Add("player swapped to " + name);
            }
            else if (gameManager.ActivePlayerCreature == null && IsAlive)
            {
                gameManager.CurrentPlayerCreatureSet(name);
            }
            else
            {
                gameManager.logs.Add("this creature is not alive");
            }

            UpdateBattleUI();
            // After attack, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void Back_S_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        private void GroundCrashers_Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroundCrasherWindow crasherWindow = new GroundCrasherWindow(gameManager, this);
            crasherWindow.Show();
            RefreshLogBox();
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            RefreshLogBox();
            this.Close();
        }
    }
}