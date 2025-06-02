using DocumentFormat.OpenXml.Drawing;
using groundCrashers_game.classes;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static groundCrashers_game.classes.Manager;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for LevelMapWindow.xaml
    /// </summary>
    public partial class LevelMapWindow : Window
    {

        // to get current biome and level index

        public int currentBiomeIndex { get; set; } = 0;
        public int currentLVLIndex { get; set; } = 1;

        bool lvls_Hidden = false;

        bool spaceMap_hidden = true;

        public LevelMapWindow(bool LVLWon = false, string LVLname = "")
        {
            InitializeComponent();

            SpaceBtn.Visibility = Visibility.Collapsed;

            GetPlayedLevelResult(LVLWon, LVLname);

            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            LVLWon = false;

            AccountManager.UpdateActiveAccount();

            if (currentBiomeIndex >= 16)
            {
                SpaceBtn.Visibility = Visibility.Visible;
            }

            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("map.wav", true);

            Display_Levels();

            Show_Biomes_Earth();
        }

        private void GetPlayedLevelResult(bool LVLWon, string LVLname)
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                // get the name of the level
                string[] LVLname_split = LVLname.Split("LVL");
                if (LVLname_split[0] == biome.ToString())
                {
                    // get biome info
                    int biomeValue = (int)biome;
                    int BiomeLVL = int.Parse(LVLname_split[1]);
                    // if lvl won and the biome and lvl have not been completed yet
                    if (LVLWon && biomeValue == currentBiomeIndex && BiomeLVL == currentLVLIndex)
                    {
                        // the earned xp for completing a level
                        int xpEarned = 1;

                        // if completed biome is 5, 10, 15, etc. you get 1 xp more
                        for (int i = 5; i <= currentBiomeIndex; i += 5)
                        {
                            xpEarned += 1;
                        }

                        // lvl go up and if lvl is 6, go to next biome
                        ActiveAccount.Active_current_biome_lvl_id++;
                        if (ActiveAccount.Active_current_biome_lvl_id == 6)
                        {
                            ActiveAccount.Active_current_biome_lvl_id = 1;
                            ActiveAccount.Active_current_biome_id++;
                            xpEarned *= 2; // Extra XP for completing a biome
                        }

                        // update the players XP
                        ActiveAccount.Active_XP += xpEarned;

                        // for every level you need 10 more xp to level up
                        int xpneeded = 10 * ActiveAccount.Active_LVL;

                        // if the player has enough XP, level up
                        if (ActiveAccount.Active_XP >= xpneeded)
                        {
                            ActiveAccount.Active_LVL++;
                            ActiveAccount.Active_XP -= xpneeded; // Reset XP after leveling up
                        }

                    }
                }
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }

        private void DisplayMapButton_Click(object sender, RoutedEventArgs e)
        {
            lvls_Hidden = !lvls_Hidden;

            string currentImage = MapBackground.ImageSource.ToString();

            string Image = currentImage.Replace("pack://application:,,,/Images/battleGrounds/", "");

            if (lvls_Hidden)
            {
                foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
                {
                    int biomeValue = (int)biome;

                    var buttonName = $"{biome}Button";
                    var button = this.FindName(buttonName) as UIElement;
                    if (button != null)
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else if (Image == "map.png")
            {
                Show_Biomes_Earth();
            }
            else if (Image == "space.png")
            {
                show_Biomes_space();
            }         
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string levelName = clicked.Name;

            // Get biome name from levelName (e.g., ForestLVL1 -> Forest)
            string biomeName = levelName.Replace("LVL1", "").Replace("Button", "");

            // Only show story window for the first level of each biome
            if (levelName.EndsWith("LVL1"))
            {
                StoryWindow storyWindow = new StoryWindow(biomeName, levelName);
                storyWindow.Show();
                this.Close();
            }
            else
            {
                GameWindow gameWindow = new GameWindow(true, levelName);
                gameWindow.Show();
                this.Close();
            }
        }

        private void SpaceBtn_Click(object sender, RoutedEventArgs e)
        {
            spaceMap_hidden = !spaceMap_hidden;
            lvls_Hidden = false;

            string currentImage = MapBackground.ImageSource.ToString();

            string newImage = currentImage.Contains("space.png")
                ? "pack://application:,,,/Images/battleGrounds/map.png"
                : "pack://application:,,,/Images/battleGrounds/space.png";

            MapBackground.ImageSource = new BitmapImage(new Uri(newImage));

            show_Biomes_space();
        }

        private void BiomeBtn_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string buttonFullName = clicked.Name;
            string biomeName = buttonFullName.Replace("Button", "");

            if (biomeName != null)
            {
                var popupName = $"{biomeName}Popup";
                var popup = this.FindName(popupName) as Popup;
                if (popup != null)
                {
                    popup.IsOpen = !popup.IsOpen;
                }
            }
        }



        private void Display_Levels()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                // make the enum value an int
                int biomeValue = (int)biome;
                // get the button name from the enum value
                var buttonBiomeName = $"{biome}Button";
                var buttonBiome = this.FindName(buttonBiomeName) as Button;
                if (buttonBiome != null)
                {
                    // hide the button
                    buttonBiome.Visibility = Visibility.Collapsed;
                    // hide all levels for this biome exept the first one
                    for (int i = 1; i <= 5; i++)
                    {
                        var buttonLVLName = $"{biome}LVL{i}";
                        var buttonLVL = this.FindName(buttonLVLName) as Button;
                        if (buttonLVL != null)
                        {
                            buttonLVL.Visibility = Visibility.Collapsed;
                        }
                    }

                    if (biomeValue == currentBiomeIndex)
                    {
                        // all levels below the current level index are green
                        for (int i = 1; i <= currentLVLIndex; i++)
                        {
                            var buttonLVLName = $"{biome}LVL{i}";
                            var buttonLVL = this.FindName(buttonLVLName) as Button;
                            if (buttonLVL != null)
                            {
                                if (i != currentLVLIndex)
                                {
                                    buttonLVL.Background = Brushes.Green;
                                    buttonLVL.BorderBrush = Brushes.LightGreen;
                                }
                                buttonLVL.Visibility = Visibility.Visible;
                            }
                        }
                    }
                    // if the biome is below the current biome, show all levels and make them greens
                    else if (biomeValue < currentBiomeIndex)
                    {
                        for (int i = 1; i <= 5; i++)
                        {
                            var buttonLVLName = $"{biome}LVL{i}";
                            var buttonLVL = this.FindName(buttonLVLName) as Control;

                            if (buttonLVL != null)
                            {
                                buttonLVL.Visibility = Visibility.Visible;
                                buttonLVL.Background = Brushes.Green;
                                buttonLVL.BorderBrush = Brushes.LightGreen;
                            }
                        }
                    }
                }
            }
        }

        private void show_Biomes_space()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {

                int biomeValue = (int)biome;
                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;
                if (button != null)
                {
                    if (spaceMap_hidden && biomeValue <= 15 && biomeValue <= currentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else if (!spaceMap_hidden && biomeValue >= 16 && biomeValue <= 24 && biomeValue <= currentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        private void Show_Biomes_Earth()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {

                int biomeValue = (int)biome;
                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;
                if (button != null)
                {
                    if (biomeValue <= 15 && biomeValue <= currentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
