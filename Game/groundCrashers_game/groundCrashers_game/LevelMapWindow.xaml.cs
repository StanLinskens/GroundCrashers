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

        bool hardcore = false;

        public LevelMapWindow(bool LVLWon = false, string LVLname = "", bool playedHardcore = false)
        {
            InitializeComponent();

            SpaceBtn.Visibility = Visibility.Collapsed;
            HardcoreBtn.Visibility = Visibility.Collapsed;



            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            GetPlayedLevelResult(LVLWon, LVLname, playedHardcore);

            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            LVLWon = false;

            AccountManager.UpdateActiveAccount();

            if (currentBiomeIndex >= 16)
            {
                SpaceBtn.Visibility = Visibility.Visible;
            }
            if (currentBiomeIndex >= 25)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }

            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("map.wav", true);

            Display_Levels();

            Show_Biomes();
        }

        private void GetPlayedLevelResult(bool LVLWon, string LVLname, bool playedHardcore)
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

                    int newActiveCurrentBiomeIndex = ActiveAccount.Active_current_biome_id;
                    if (playedHardcore) newActiveCurrentBiomeIndex -= 25;

                    if (LVLWon && biomeValue == newActiveCurrentBiomeIndex && BiomeLVL == currentLVLIndex)
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

                        bool completedNewEarth = ActiveAccount.Active_current_biome_lvl_id >= 6 && newActiveCurrentBiomeIndex <= 15;
                        bool completedNewSpace = ActiveAccount.Active_current_biome_lvl_id >= 4 && newActiveCurrentBiomeIndex >= 16 && newActiveCurrentBiomeIndex <= 24;
                        if (completedNewEarth || completedNewSpace)
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
            else
            {
                Show_Biomes();
            }   
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            string levelName = clicked.Name;

            // Get biome name from levelName (e.g., ForestLVL1 -> Forest)
            string biomeName = levelName.Replace("LVL1", "").Replace("Button", "");

            string currentImage = MapBackground.ImageSource.ToString();
            bool isHardcore = currentImage.Contains("hardcore");

            // Only show story window for the first level of each biome
            if (levelName.EndsWith("LVL1"))
            {
                StoryWindow storyWindow = new StoryWindow(biomeName, levelName, isHardcore);
                storyWindow.Show();
                this.Close();
            }
            else
            {
                GameWindow gameWindow = new GameWindow(true, levelName, isHardcore);
                gameWindow.Show();
                this.Close();
            }
        }

        private void SpaceBtn_Click(object sender, RoutedEventArgs e)
        {
            spaceMap_hidden = !spaceMap_hidden;
            lvls_Hidden = false;
            hardcore = false;

            string newImage = string.Empty;

            string currentImage = MapBackground.ImageSource.ToString();

            if(currentImage.Contains("space.png"))
            {
                newImage = "pack://application:,,,/Images/battleGrounds/map.png";
                SpaceBtn.Content = "Space Map";
            }
            else
            {
                newImage = "pack://application:,,,/Images/battleGrounds/space.png";
                SpaceBtn.Content = "Earth Map";
            }

            MapBackground.ImageSource = new BitmapImage(new Uri(newImage));

            Show_Biomes();

            if (!spaceMap_hidden && currentBiomeIndex - 24 >= 16)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else if (!spaceMap_hidden)
            {
                HardcoreBtn.Visibility = Visibility.Collapsed;
            }
            else
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
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

        private void HardcoreBtn_Click(object sender, RoutedEventArgs e)
        {
            hardcore = !hardcore;
            lvls_Hidden = false;

            string currentImage = MapBackground.ImageSource.ToString();
            if (spaceMap_hidden)
            {
                string newImage = currentImage.Contains("hardcore_map.png")
                ? "pack://application:,,,/Images/battleGrounds/map.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_map.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            else
            {
                string newImage = currentImage.Contains("hardcore_space.png")
                ? "pack://application:,,,/Images/battleGrounds/space.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_space.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }

            Display_Levels();
            Show_Biomes();
        }

        private void Display_Levels()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {

                int newCurrentBiomeIndex = currentBiomeIndex;

                if (hardcore) { newCurrentBiomeIndex -= 25; }

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
                            buttonLVL.Background = new SolidColorBrush(Color.FromRgb(0x1E, 0x1E, 0x1E));  // #1E1E1E
                            buttonLVL.BorderBrush = new SolidColorBrush(Color.FromRgb(0x44, 0x44, 0x44)); // #444444
                        }
                    }

                    if (biomeValue == newCurrentBiomeIndex)
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
                    else if (biomeValue < newCurrentBiomeIndex)
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

        private void Show_Biomes()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {

                int biomeValue = (int)biome;
                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;

                int newCurrentBiomeIndex = currentBiomeIndex;

                if (hardcore) { newCurrentBiomeIndex -= 25; }

                if (button != null)
                {
                    if (spaceMap_hidden && biomeValue <= 15 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else if (!spaceMap_hidden && biomeValue >= 16 && biomeValue <= 24 && biomeValue <= newCurrentBiomeIndex)
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

        private void CapsuleBtn_Click(object sender, RoutedEventArgs e)
        {
            GambleWindow gambleWindow = new GambleWindow(); 
            gambleWindow.Show();
        }
    }
}
