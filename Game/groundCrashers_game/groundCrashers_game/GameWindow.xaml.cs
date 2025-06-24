using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using groundCrashers_game.classes;
using Microsoft.VisualBasic.Logging;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Xml.Linq;
using WMPLib;
using static groundCrashers_game.classes.Manager;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        Manager gameManager;
        Esp32Manager portalManager;

        private Button fightButton;
        private Button bagButton;
        private Button groundCrashersButton;
        private Button runButton;
        private WrapPanel actionButtonsPanel;

        // link the elements to a color for later use
        private Dictionary<Elements, string> CreatureElementColor = new()
        {
            { Elements.Nature, "#6AA84F" },     
            { Elements.Ice, "#66CCFF" },        
            { Elements.Toxic, "#9C27B0" },      
            { Elements.Fire, "#FF5722" },       
            { Elements.Water, "#2196F3" },      
            { Elements.Draconic, "#B71C1C" },   
            { Elements.Earth, "#8D6E63" },      
            { Elements.Dark, "#888888" },       
            { Elements.Wind, "#26C6DA" },       
            { Elements.Psychic, "#EC407A" },    
            { Elements.Light, "#FFE066" },      
            { Elements.Demonic, "#880E4F" },    
            { Elements.Electric, "#FFEB3B" },   
            { Elements.Acid, "#AEEA00" },       
            { Elements.Magnetic, "#9E9E9E" },   
            { Elements.Cosmic, "#7B1FA2" },     
            { Elements.Chaos, "#C2185B" },      
            { Elements.Void, "#424242" },       
            { Elements.Astral, "#CEB4FF" }      
        };

        // link the primaries to a color for later use
        private Dictionary<Primaries, string> CreaturePrimaryColor = new()
        {
            { Primaries.Verdant, "#388E3C" },   
            { Primaries.Primal, "#FFB300" },    
            { Primaries.Apex, "#C62828" },      
            { Primaries.Sapient, "#9C27B0" },   
            { Primaries.Synthetic, "#2196F3" }, 
            { Primaries.God, "#333" },       
            { Primaries.Titan, "#B71C1C" }      
        };

        /// <summary>
        /// the gamewindow where all the magic for the game happens.
        /// </summary>
        /// <param name="storyMode">if storymode or not</param>
        /// <param name="LVLName">what level it is or null as default</param>
        /// <param name="hardcore">if hardcore or not</param>
        public GameWindow(bool storyMode, string LVLName = "null", bool hardcore = false)
        {
            InitializeComponent();

            // Initialize the game manager
            gameManager = new Manager();

            gameManager.hardcore = hardcore; // Set hardcore mode if applicable
            gameManager.StoryMode = storyMode; // set story mode if applicable
            gameManager.levelName = LVLName; // set the level name if applicable

            // set the default img
            WinLoseImage.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/youlose.png", UriKind.Absolute));
            WinLoseOverlay.Visibility = Visibility.Collapsed;
            WinLoseImage.IsEnabled = false;

            // pull the data
            gameManager.LoadGameData();
            // load the actors for the battle mode
            gameManager.LoadActorsForBattleMode();

            // Initialize the portal manager
            portalManager = new Esp32Manager();

            // Find the WrapPanel in the XAML layout
            FindActionButtonsPanel();

            // if story mode is enabled, generate the environment based on the level
            if (storyMode)
            {
                EnviromentGenerator();

                if (Levels.Chart.TryGetValue(gameManager.levelName, out var level))
                {
                    gameManager._maxCreatures = level.AmountCreaturesPlayer; // Use the 'level' object to access the property
                }
            }
            else
            {
                RandomScenarioGenerator(); // Generate a random scenario if not in story mode
            }

            // play the random battle music
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlayRandomBattleMusic();

            // refresh the log box
            RefreshLogBox();
        }

        /// <summary>
        /// this method refreshes the log box with the latest logs from the game manager. 
        /// and updates the logbox text with the latest logs.
        /// </summary>
        public void RefreshLogBox()
        {
            gameManager.ControlLogs();
            logbox.Text = string.Join("\n", gameManager.logs);
        }

        /// <summary>
        /// update the battle UI with the current player and CPU creature stats.
        /// </summary>
        public void UpdateBattleUI()
        {
            // get the active player creature and update the UI elements accordingly
            var playerCreature = gameManager.ActivePlayerCreature;
            if (playerCreature != null)
            {
                PlayerCreatureName.Content = playerCreature.name;
                PlayerHealthText.Text = playerCreature.stats.hp.ToString() + "/" + playerCreature.stats.max_hp.ToString();
                PlayerHealthBar.Value = playerCreature.stats.hp;
                PlayerHealthBar.Maximum = playerCreature.stats.max_hp;
                PlayerCreatureName.Foreground = new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(
                        CreaturePrimaryColor.GetValueOrDefault(
                            playerCreature.primary_type, 
                            "#555555")));
                PlayerCreatureName.BorderBrush =
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
                    // set the img
                    PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{playerCreature.name}.png", UriKind.Absolute));

                    // show attack like sword/elemnt/defend img
                    ActionDisplayPlayer();
                }
                catch (Exception ex)
                {
                    // if there was a error set the default img
                    PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                    PlayerChoise.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }

            }
            // if player has none selected
            else
            {
                // check if actiondisplayplayer can be called, if not catch the error
                try { ActionDisplayPlayer(); }
                catch { PlayerChoise.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute)); } // Just to avoid any null reference issues

                // reset the player choise to none
                gameManager.playerChoise = "none";

                // reset the player img to default
                PlayerImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));

                // update player health bar and text
                string PlayerHealthT = PlayerHealthText.Text.ToString();
                string[] PlayerHealth_split = PlayerHealthT.Split('/');

                // bar to 0
                PlayerHealthBar.Value = 0;
                PlayerHealthText.Text = "0/" + PlayerHealth_split[1];

                // check if the player has any creatures left
                Creature hasCreatureLeft = gameManager.GetPlayerActor().Creatures.FirstOrDefault(c => c.alive);
                if (hasCreatureLeft == null)
                {
                    // show you lose img and log
                    gameManager.logs.Add("you Lose");
                    RefreshLogBox();
                    WinLoseImage.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/youlose.png", UriKind.Absolute));
                    WinLoseOverlay.Visibility = Visibility.Visible;
                    WinLoseImage.IsEnabled = true;
                    gameManager.Win = false;
                }

            }


            // Likewise for the CPU’s active creature:
            var cpuCreature = gameManager.ActiveCpuCreature;
            if (cpuCreature != null)
            {
                EnemyCreatureName.Content = cpuCreature.name;
                EnemyHealthText.Text = cpuCreature.stats.hp.ToString() + "/" + cpuCreature.stats.max_hp.ToString();
                EnemyHealthBar.Value = cpuCreature.stats.hp;
                EnemyHealthBar.Maximum = cpuCreature.stats.max_hp;

                EnemyCreatureName.Foreground =
                    new SolidColorBrush((Color)ColorConverter.ConvertFromString(
                        CreaturePrimaryColor.GetValueOrDefault( 
                            cpuCreature.primary_type,
                            "#555555")));
                EnemyCreatureName.BorderBrush =
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
                    // set the img
                    EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/GroundCrasherSprites/{cpuCreature.name}.png", UriKind.Absolute));

                    // dispaly the cpu action img
                    ActionDisplayCpu();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Error loading image: {ex.Message}");
                    EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                    CpuChoise.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));
                }
            }
            else
            {
                // display the cpu action img if error catch
                try { ActionDisplayCpu(); }
                catch { CpuChoise.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute)); } // Just to avoid any null reference issues

                // reset it
                gameManager.cpuChoise = "none";

                // reset the cpu img to default
                EnemyImageBox.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/questionmark.png", UriKind.Absolute));

                string EnemyHealthT = EnemyHealthText.Text.ToString();
                string[] EnemyHealt_split = EnemyHealthT.Split('/');

                EnemyHealthBar.Value = 0;
                EnemyHealthText.Text = "0/" + EnemyHealt_split[1];

                // you win if the cpu has no creatures left
                gameManager.logs.Add("you win");
                // logbox refresh
                RefreshLogBox();
                // show you win img
                WinLoseImage.Source = new BitmapImage(new Uri($"pack://application:,,,/images/other/youwin.png", UriKind.Absolute));
                WinLoseOverlay.Visibility = Visibility.Visible;
                WinLoseImage.IsEnabled = true;
                gameManager.Win = true;
            }
        }

        private void attack_Animation(string attacker_name)
        {
            // Define base rotation angles for the attack animation
            double[] angles = new double[] { 25, -30, 10, 0 };

            // Flip the angles for the enemy to mirror the animation
            if (attacker_name == "Enemy")
            {
                angles = angles.Select(a => -a).ToArray();
            }

            // Create the animation using keyframes
            var keyFrames = new DoubleAnimationUsingKeyFrames
            {
                Duration = TimeSpan.FromSeconds(0.4)
            };

            keyFrames.KeyFrames.Add(new EasingDoubleKeyFrame(angles[0], KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.05))));
            keyFrames.KeyFrames.Add(new EasingDoubleKeyFrame(angles[1], KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.2)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });
            keyFrames.KeyFrames.Add(new EasingDoubleKeyFrame(angles[2], KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.3)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            });
            keyFrames.KeyFrames.Add(new EasingDoubleKeyFrame(angles[3], KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.4)))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            });

            // Apply to the correct image
            Image attackerBox;
            if (attacker_name == "Player") {
                attackerBox = PlayerImageBox;
            } else {
                attackerBox = EnemyImageBox;
            }


            // Ensure the RenderTransform is properly set
            if (!(attackerBox.RenderTransform is TransformGroup group))
            {
                group = new TransformGroup();
                group.Children.Add(new ScaleTransform(-1, 1)); // Keep horizontal flip
                group.Children.Add(new RotateTransform(0));
                attackerBox.RenderTransform = group;
            }

            // Ensure there's a RotateTransform present
            RotateTransform rotateTransform = group.Children.OfType<RotateTransform>().FirstOrDefault();
            if (rotateTransform == null)
            {
                rotateTransform = new RotateTransform(0);
                group.Children.Add(rotateTransform);
            }

            // Create and start the animation storyboard
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(keyFrames);
            Storyboard.SetTarget(keyFrames, attackerBox);
            Storyboard.SetTargetProperty(keyFrames, new PropertyPath("RenderTransform.Children[1].(RotateTransform.Angle)"));
            storyboard.Begin();
        }

        /// <summary>
        /// this displays the player's action in the battle UI.
        /// </summary>
        private void ActionDisplayPlayer()
        {
            // If the player made an action, display it
            if (gameManager.playerChoise != "none" && gameManager.playerChoise != "swap")
            {
                // If the player made an attack or elemental attack, play the animation
                if (gameManager.playerChoise == "attack" || gameManager.playerChoise == "elementattack")
                {
                    attack_Animation(attacker_name: "Player");
                }
                // make the img visable
                PlayerChoise.Visibility = Visibility.Visible;
                // update img to most recent action
                string location = $"pack://application:,,,/images/other/{gameManager.playerChoise}.png";
                PlayerChoise.Source = new BitmapImage(new Uri(location, UriKind.Absolute));
                gameManager.playerChoise = "none"; // Reset after displaying
            }
            // If the player did not make an action, hide the image
            else
            {
                PlayerChoise.Visibility = Visibility.Hidden; // hide if the action is none or swap
            }
        }

        /// <summary>
        /// display the action of the CPU in the battle UI.
        /// </summary>
        private void ActionDisplayCpu()
        {
            // If the CPU made an action, display it
            if (gameManager.cpuChoise != "none" && gameManager.cpuChoise != "swap")
            {
                // If the CPU made an attack or elemental attack, play the animation
                if (gameManager.cpuChoise == "attack" || gameManager.cpuChoise == "elementattack")
                {
                    attack_Animation(attacker_name: "Enemy");
                }
                // make the img visable
                CpuChoise.Visibility = Visibility.Visible;
                // chance the img to the most recent action
                string location = $"pack://application:,,,/images/other/{gameManager.cpuChoise}CPU.png";
                CpuChoise.Source = new BitmapImage(new Uri(location, UriKind.Absolute));
                gameManager.cpuChoise = "none"; // Reset after displaying
            }
            // If the CPU did not make an action, hide the image
            else
            {
                // if not an action, hide the image
                CpuChoise.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// generate the environment based on the current level or scenario.
        /// </summary>
        private void EnviromentGenerator()
        {
            // 1) Get enum values
            var Biome = gameManager.GetBiome();
            var Time = gameManager.GetDaytime();
            var randomWeather = Manager.GetRandomWeather();
            // 2) Update the TextBlocks
            BiomeText.Text = Biome.ToString().ToUpper();
            BiomeIcon.Text = GetBiomeEmoji(Biome);
            DaytimeText.Text = Time.ToString().ToUpper();
            DaytimeIcon.Text = GetDaytimeEmoji(Time);
            WeatherText.Text = randomWeather.ToString().ToUpper();
            WeatherIcon.Text = GetWeatherEmoji(randomWeather);
            // 3) Set the background image based on the random biome
            try
            {
                BiomeBackground.ImageSource = new BitmapImage(new Uri($"pack://application:,,,/images/battleGrounds/{Biome.ToString().ToLower()}.jpg", UriKind.Absolute));
            }
            catch
            {
                MessageBox.Show("Error loading biome");
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
        }

        /// <summary>
        /// map each Biome to a simple emoji
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private string GetBiomeEmoji(Biomes b)
        {
            switch (b)
            {
                // normal biomes
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
                case Biomes.Ruins: return "🏰";
                case Biomes.Marsh: return "🦢";
                case Biomes.CrystalCavern: return "💎";
                case Biomes.Wasteland: return "☢️";

                // underground-themed biomes
                case Biomes.Catacomb: return "⚰️";
                case Biomes.LavaChamber: return "🔥";
                case Biomes.CaveCitadel: return "🏰";
                case Biomes.Altar: return "🛐";
                case Biomes.CaveLake: return "🏞️";
                case Biomes.CaveVillage: return "🏡";
                case Biomes.FungalHollow: return "🍄";
                case Biomes.Dungeon: return "🗝️";

                // marine-themed biomes
                case Biomes.Estuaries: return "🏝️";
                case Biomes.CoralReef: return "🪸";
                case Biomes.OpenOcean: return "🌊";
                case Biomes.DeepCoralReef: return "🪸";
                case Biomes.ColdSeep: return "🧊";
                case Biomes.HydroVent: return "🌋";

                // Space-themed biomes
                case Biomes.Earth: return "🌍";
                case Biomes.Moon: return "🌕";
                case Biomes.Nebula: return "🌀";
                case Biomes.Interstellar: return "🌌";
                case Biomes.Debris: return "🛰️";
                case Biomes.Saturn: return "🪐";
                case Biomes.Cybertron: return "🤖";
                case Biomes.Asteroids: return "☄️";
                case Biomes.Sun: return "☀️";

                // might add later
                //case Biomes.Skyloft: return "☁️";

                // else
                default: return "❓";
            }
        }

        /// <summary>
        /// map each Daytime to an emoji
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
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

        /// <summary>
        /// map each Weather to an emoji
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get the action buttons panel from the XAML layout.
        /// </summary>
        private void FindActionButtonsPanel()
        {
            // This method locates the WrapPanel that contains the action buttons in the UI.
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

        // store the references to the original action buttons for later use
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

        /// <summary>
        /// if the fight button is clicked, it will check if the player has a creature selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// show the combat options when the fight button is clicked.
        /// </summary>
        private void ShowCombatOptions()
        {
            UpdateBattleUI();

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

        /// <summary>
        /// create buttons place
        /// </summary>
        /// <param name="content">the content</param>
        /// <param name="background">the background color</param>
        /// <param name="border">the border</param>
        /// <param name="clickHandler">the clickhandeler</param>
        /// <returns></returns>
        private Button CreateButton(string content, string background, string border, RoutedEventHandler clickHandler)
        {
            Button button = new Button
            {
                Content = content,
                Height = 60,
                Width = 400,
                Margin = new System.Windows.Thickness(10),
                FontSize = 20,
                Background = (Brush)new BrushConverter().ConvertFromString(background),
                BorderBrush = (Brush)new BrushConverter().ConvertFromString(border),
                Style = (System.Windows.Style)FindResource("DarkButton"), // Apply the shared DarkButton style
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

        /// <summary>
        /// if attack button is clicked, it will process the turn with an attack action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Attack_Button_Click(object sender, RoutedEventArgs e)
        {
            gameManager.ProcessTurn(ActionType.Attack);
            UpdateBattleUI();
            // After attack, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        /// <summary>
        /// if the element button is clicked, it will process the turn with an elemental attack action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Element_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement elemental attack logic
            gameManager.ProcessTurn(ActionType.ElementAttack);
            UpdateBattleUI();
            // After attack, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        /// <summary>
        /// if the defend button is clicked, it will process the turn with a defend action.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Defend_Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement defense logic
            gameManager.ProcessTurn(ActionType.Defend);

            UpdateBattleUI();
            // After defense, restore main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        /// <summary>
        /// if the back button is clicked, it will restore the main action buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        /// <summary>
        /// restore the main action buttons to the action buttons panel
        /// </summary>
        private void RestoreMainActionButtons()
        {
            actionButtonsPanel.Children.Clear();

            // add them to the action button pannel if it is not null
            if (fightButton != null) actionButtonsPanel.Children.Add(fightButton);
            if (bagButton != null) actionButtonsPanel.Children.Add(bagButton);
            if (groundCrashersButton != null) actionButtonsPanel.Children.Add(groundCrashersButton);
            if (runButton != null) actionButtonsPanel.Children.Add(runButton);
        }

        /// <summary>
        /// when the swap button is clicked, it will check if the player has enough creatures to swap.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Swap_Button_Click(object sender, RoutedEventArgs e)
        {
            Actor playerActor = gameManager.GetPlayerActor();
            if (playerActor.Creatures.Count < gameManager._maxCreatures)
            {
                gameManager.logs.Add($"You need at least {gameManager._maxCreatures} creatures to swap.");
            }
            else
            {
                // update the battle UI
                UpdateBattleUI();
                //Replace the current buttons with swap options
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

            // get the player actor
            Actor playerActor = gameManager.GetPlayerActor();

            // go trough the creatures and create buttons for each creature. only if alive create a button
            foreach (Creature c in playerActor.Creatures)
            {
                if(c.alive)
                {
                    Button Creature = CreateButton(c.name.ToString() ?? "Not Found", CreaturePrimaryColor.GetValueOrDefault(c.primary_type) ?? "#555555", CreatureElementColor.GetValueOrDefault(c.element) ?? "#555555", Creature_Button_Click);
                    actionButtonsPanel.Children.Add(Creature);
                }
            }

            // back button to return to main action buttons
            Button back_S_Button = CreateButton("BACK", "Gray", "DarkGray", Back_S_Button_Click);

            // Add the new buttons to the panel
            actionButtonsPanel.Children.Add(back_S_Button);
        }

        /// <summary>
        /// when player pressed on creature button, it will swap the active creature with the selected one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Creature_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button clicked = sender as Button;
                string name = clicked.Content.ToString() ?? "default";

                // get the player actor and find the creature by name
                Actor playerActor = gameManager.GetPlayerActor();
                var creature = playerActor.Creatures.FirstOrDefault(c => c.name == name);

                // if it is null, it means the creature was not found
                if (creature == null)
                {
                    gameManager.logs.Add("Creature not found.");
                }
                // if it is dead it cant swap to it
                else if (!creature.alive)
                {
                    gameManager.logs.Add("This creature is dead.");
                }
                // if the creature is alive and not the active player creature, swap to it
                else if (gameManager.ActivePlayerCreature != null && gameManager.ActivePlayerCreature.name != name)
                {
                    gameManager.ProcessTurn(ActionType.Swap, name);
                    gameManager.logs.Add("Player swapped to " + name);
                }
                // if the active player creature is null, set it to the selected creature without processing a turn
                else if (gameManager.ActivePlayerCreature == null)
                {
                    gameManager.CurrentPlayerCreatureSet(name);
                }
                // if the creature is already active, do nothing
                else
                {
                    gameManager.logs.Add("This creature is already active.");
                }

                UpdateBattleUI();
                RestoreMainActionButtons();
                RefreshLogBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Swap failed: " + ex.Message);
            }
        }

        /// <summary>
        /// restore the main action buttons when the back button is clicked in the swap options.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_S_Button_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main action buttons
            RestoreMainActionButtons();
            RefreshLogBox();
        }

        /// <summary>
        /// show the groundcrashers window when the groundcrashers button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroundCrashers_Button_Click_2(object sender, RoutedEventArgs e)
        {
            GroundCrasherWindow crasherWindow = new GroundCrasherWindow(gameManager, this, portalManager);
            crasherWindow.ShowDialog();
            RefreshLogBox();
        }

        /// <summary>
        /// leave the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameManager.StoryMode)
            {
                LevelMapWindow MapWindow = new LevelMapWindow(false, gameManager.levelName, gameManager.hardcore);
                MapWindow.Show();
                RefreshLogBox();
                this.Close();
            }
            else
            {
                MainWindow MainWindow = new MainWindow();
                MainWindow.Show();
                RefreshLogBox();
                this.Close();
            }

        }

        /// <summary>
        /// shwo the stats of the player creature when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayerCreatureName_Click(object sender, RoutedEventArgs e)
        {
            // get information about the player creature and display it in the logbox
            List<string> newLogs = new List<string>();
            Actor PlayerActor = gameManager.GetPlayerActor();
            // go trough all the creatures of the player actor and find the one that matches the PlayerCreatureName
            foreach (Creature c in PlayerActor.Creatures)
            {
                if (c.name == PlayerCreatureName.Content.ToString())
                {
                    // dispaly the information in the logbox
                    newLogs.Add($"{c.name} (Player)");
                    newLogs.Add($"Primary Type: {c.primary_type}");
                    newLogs.Add($"Element: {c.element}");
                    newLogs.Add($"Curse: {c.curse}");

                    // go trough all the creatures and find the one that matches the PlayerCreatureName
                    foreach (Creature all_c in gameManager.AllCreatures)
                    {
                        // display the stats of the player creature and the normal stats without the buffs/curse
                        if (all_c.name == PlayerCreatureName.Content.ToString())
                        {
                            newLogs.Add($"current hp: {c.stats.hp} max hp: {c.stats.max_hp} normal max hp: {all_c.stats.hp * 3}");
                            newLogs.Add($"current attack: {c.stats.attack} max attack: {c.stats.max_attack} normal attack: {all_c.stats.attack}");
                            newLogs.Add($"current defense: {c.stats.defense} max defense: {c.stats.max_defense} normal defense: {all_c.stats.defense}");
                            newLogs.Add($"current speed: {c.stats.speed} max speed: {c.stats.max_speed} normal speed: {all_c.stats.speed}");
                        }
                    }

                    // set the logbox text to the new logs
                    logbox.Text = string.Join("\n", newLogs);
                    newLogs.Clear();
                    return;
                }
            }
            // if the creature was not found, add a log to the game manager logs
            gameManager.logs.Add("Creature not found.");
            RefreshLogBox();
        }

        /// <summary>
        /// show the stats of the enemy creature when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnemyCreatureName_Click(object sender, RoutedEventArgs e)
        {
            // get information about the cpu creature and display it in the logbox
            List<string> newLogs = new List<string>();
            Actor CpuActor = gameManager.GetCpuActor();
            // go trough all the creatures of the player actor and find the one that matches the PlayerCreatureName
            foreach (Creature c in CpuActor.Creatures)
            {
                if (c.name == EnemyCreatureName.Content.ToString())
                {
                    // dispaly the information in the logbox
                    newLogs.Add($"{c.name} (CPU)");
                    newLogs.Add($"Primary Type: {c.primary_type}");
                    newLogs.Add($"Element: {c.element}");
                    newLogs.Add($"Curse: {c.curse}");

                    // go trough all the creatures and find the one that matches the PlayerCreatureName
                    foreach (Creature all_c in gameManager.AllCreatures)
                    {
                        // display the stats of the player creature and the normal stats without the buffs/curse
                        if (all_c.name == EnemyCreatureName.Content.ToString())
                        {
                            newLogs.Add($"current hp: {c.stats.hp} max hp: {c.stats.max_hp} normal max hp: {all_c.stats.hp * 3}");
                            newLogs.Add($"current attack: {c.stats.attack} max attack: {c.stats.max_attack} normal attack: {all_c.stats.attack}");
                            newLogs.Add($"current defense: {c.stats.defense} max defense: {c.stats.max_defense} normal defense: {all_c.stats.defense}");
                            newLogs.Add($"current speed: {c.stats.speed} max speed: {c.stats.max_speed} normal speed: {all_c.stats.speed}");
                        }
                    }
                    // set the logbox text to the new logs
                    logbox.Text = string.Join("\n", newLogs);
                    newLogs.Clear();
                    return;
                }
            }
            // if the creature was not found, add a log to the game manager logs
            gameManager.logs.Add("Creature not found.");
            RefreshLogBox();
        }

        /// <summary>
        /// when the game is over and the player pressed on the img go the the main screen or the level map depending on the game mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinLoseImage_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (gameManager.StoryMode)
            {
                LevelMapWindow mapWindow = new LevelMapWindow(gameManager.Win, gameManager.levelName, gameManager.hardcore);
                mapWindow.Show();
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
            this.Close();
        }
    }
}