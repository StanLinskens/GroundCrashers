using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using groundCrashers_game.classes;
using Newtonsoft.Json;
using System.IO;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        private Manager gameManager;
        private Button fightButton;
        private Button bagButton;
        private Button groundCrashersButton;
        private Button runButton;
        private WrapPanel actionButtonsPanel;

        public GameWindow()
        {
            InitializeComponent();

            // Initialize the game manager
            gameManager = new Manager();
            //gameManager.LoadAllCreatures();

            // Find the WrapPanel in the XAML layout
            FindActionButtonsPanel();

            // 1) Get random enum values
            var randomBiome = Manager.GetRandomBiome();
            var randomTime = Manager.GetRandomDaytime();
            var randomWeather = Manager.GetRandomWeather();

            // 2) Update the TextBlocks
            BiomeText.Text = randomBiome.ToString().ToUpper();    // e.g. "FOREST"
            BiomeIcon.Text = GetBiomeEmoji(randomBiome);

            DaytimeText.Text = randomTime.ToString().ToUpper();     // e.g. "DUSK"
            DaytimeIcon.Text = GetDaytimeEmoji(randomTime);

            WeatherText.Text = randomWeather.ToString().ToUpper();  // e.g. "FOGGY"
            WeatherIcon.Text = GetWeatherEmoji(randomWeather);
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
                case Biomes.Crystal_Cavern: return "💎";
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
                case Weathers.Clear: return "🆓";   // or "🔆"
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
                        case "BAG":
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
            // Replace the current buttons with combat options
            ShowCombatOptions();
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
            Button attackButton = CreateCombatButton("ATTACK", "#591C1C", "#7A2929", Attack_Button_Click);
            Button elementButton = CreateCombatButton("ELEMENT", "#594C1C", "#7A6929", Element_Button_Click);
            Button defendButton = CreateCombatButton("DEFEND", "#1C591C", "#297A29", Defend_Button_Click);
            Button backButton = CreateCombatButton("BACK", "Gray", "DarkGray", Back_Button_Click);

            // Add the new buttons to the panel
            actionButtonsPanel.Children.Add(attackButton);
            actionButtonsPanel.Children.Add(elementButton);
            actionButtonsPanel.Children.Add(defendButton);
            actionButtonsPanel.Children.Add(backButton);
        }

        private Button CreateCombatButton(string content, string background, string border, RoutedEventHandler clickHandler)
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
            // Implement elemental attack logic
            MessageBox.Show("Attack selected!");
            // After attack, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Element_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement elemental attack logic
            MessageBox.Show("Element attack selected!");
            // After attack, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Defend_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement defense logic
            MessageBox.Show("Defense selected!");
            // After defense, restore main action buttons
            RestoreMainActionButtons();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
        }

        private void RestoreMainActionButtons()
        {
            actionButtonsPanel.Children.Clear();

            if (fightButton != null) actionButtonsPanel.Children.Add(fightButton);
            if (bagButton != null) actionButtonsPanel.Children.Add(bagButton);
            if (groundCrashersButton != null) actionButtonsPanel.Children.Add(groundCrashersButton);
            if (runButton != null) actionButtonsPanel.Children.Add(runButton);
        }


        private void Bag_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement bag functionality
            MessageBox.Show("Bag selected!");
        }

        private void GroundCrashers_Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroundCrasherWindow GroundCrasherWindow = new GroundCrasherWindow();
            GroundCrasherWindow.Show();
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }
    }
}