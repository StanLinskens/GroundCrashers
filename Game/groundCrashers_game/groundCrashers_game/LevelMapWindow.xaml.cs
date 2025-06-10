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

        private bool lvls_Hidden = false;

        private bool hardcore = false;

        private int AmountofBiomes_Map = 15; // 16 map biomes (0-15)

        private int AmountofBiomes_Space = 9; // 9 space biomes (22-30) (30)

        private int AmountofBiomes_Marine = 6;

        private int AmountofBiomes = 38; // 16 map + 6 marine + 9 space + 6 underground

        GambleWindow gambleWindow;

        public LevelMapWindow(bool LVLWon = false, string LVLname = "", bool playedHardcore = false)
        {
            GambleWindow gambleWindow = new GambleWindow();
            InitializeComponent();

            MapComboBox.Visibility = Visibility.Collapsed;
            HardcoreBtn.Visibility = Visibility.Collapsed;

            GetPlayedLevelResult(LVLWon, LVLname, playedHardcore);

            LVLWon = false;

            AccountManager.UpdateActiveAccount();

            if (currentBiomeIndex >= 6 && currentBiomeIndex <= 20)
            {
                MapComboBox.Visibility = Visibility.Visible;
                MapComboBox_Earth.Visibility = Visibility.Visible;
                MapComboBox_Cave.Visibility = Visibility.Visible;
                MapComboBox_Marine.Visibility = Visibility.Collapsed;
                MapComboBox_Space.Visibility = Visibility.Collapsed;
            }
            else if (currentBiomeIndex >= 21 && currentBiomeIndex <= 28 )
            {
                MapComboBox.Visibility = Visibility.Visible;
                MapComboBox_Earth.Visibility = Visibility.Visible;
                MapComboBox_Cave.Visibility = Visibility.Visible;
                MapComboBox_Marine.Visibility = Visibility.Visible;
                MapComboBox_Space.Visibility = Visibility.Collapsed;
            }
            else if (currentBiomeIndex >= 29)
            {
                MapComboBox.Visibility = Visibility.Visible;
                MapComboBox_Earth.Visibility = Visibility.Visible;
                MapComboBox_Cave.Visibility = Visibility.Visible;
                MapComboBox_Marine.Visibility = Visibility.Visible;
                MapComboBox_Space.Visibility = Visibility.Visible;
            }

            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("map.wav", true);

            // Default to Earth map
            // because of this the MapComboBox_SelectionChanged event triggers
            if(MapComboBox.SelectedIndex == -1) MapComboBox.SelectedIndex = 0;
        }

        private void GetPlayedLevelResult(bool LVLWon, string LVLname, bool playedHardcore)
        {

            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            bool completedEarth = false;
            bool completedCave = false;
            bool completedMarine = false;
            bool completedSpace = false;

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

                    int CoinsEarned = 0;

                    // the earned xp for completing a level
                    int xpEarned = 0;

                    if(LVLWon)
                    {
                        xpEarned = 1;
                        CoinsEarned = 1; // 1 coin for winning the level
                        // if completed biome is 5, 10, 15, etc. you get 1 xp more
                        for (int i = 5; i <= currentBiomeIndex; i += 5)
                        {
                            xpEarned += 1;
                        }
                    }


                    int newActiveCurrentBiomeIndex = ActiveAccount.Active_current_biome_id;
                    if (playedHardcore) { newActiveCurrentBiomeIndex -= AmountofBiomes; CoinsEarned += 2; }

                    completedEarth = ((newActiveCurrentBiomeIndex <= 5) || (newActiveCurrentBiomeIndex >= 14 && newActiveCurrentBiomeIndex <= 20) || (newActiveCurrentBiomeIndex >= 27 && newActiveCurrentBiomeIndex <= 28));
                    completedCave = newActiveCurrentBiomeIndex >= 6 && newActiveCurrentBiomeIndex <= 13;
                    completedMarine = newActiveCurrentBiomeIndex >= 21 && newActiveCurrentBiomeIndex <= 26;
                    completedSpace = newActiveCurrentBiomeIndex >= 29 && newActiveCurrentBiomeIndex <= 37;
                    
                    if (LVLWon && biomeValue == newActiveCurrentBiomeIndex && BiomeLVL == currentLVLIndex)
                    {
                        // lvl go up and if lvl is 6, go to next biome
                        ActiveAccount.Active_current_biome_lvl_id++;

                        bool completedNewEarth = ActiveAccount.Active_current_biome_lvl_id >= 6 && completedEarth;
                        bool completedNewCave = ActiveAccount.Active_current_biome_lvl_id >= 5 && completedCave;
                        bool completedNewMarine = ActiveAccount.Active_current_biome_lvl_id >= 5 && completedMarine;
                        bool completedNewSpace = ActiveAccount.Active_current_biome_lvl_id >= 4 && completedSpace;

                        if (completedNewEarth || completedNewMarine || completedNewSpace || completedNewCave)
                        {
                            ActiveAccount.Active_current_biome_lvl_id = 1;
                            ActiveAccount.Active_current_biome_id++;
                            xpEarned *= 2; // Extra XP for completing a biome
                        }

                        bool isLastBiomeStory = LVLname == "CrystalCavernLVL5" || LVLname == "AltarLVL4";
                        if (isLastBiomeStory)
                        {
                            StoryWindow storyWindow = new StoryWindow(LVLname, LVLname, false);
                            storyWindow.ShowDialog(); // <- this blocks the current thread until the window is closed
                        }
                    }
                    // update the players XP
                    ActiveAccount.Active_XP += xpEarned;
                    AccountManager.LevelUp();

                    ActiveAccount.Active_coins += CoinsEarned; // Add coins for winning the level
                }
            }
            currentLVL.Text = $"LVL: {ActiveAccount.Active_LVL}"; // Update the current level text
            currentXP.Text = $"XP: {ActiveAccount.Active_XP}"; // Update the current XP text

            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            Display_Levels();

            if (completedEarth) MapComboBox.SelectedIndex = 0;
            else if (completedCave) MapComboBox.SelectedIndex = 1;
            else if (completedMarine) MapComboBox.SelectedIndex = 2;
            else if (completedSpace) MapComboBox.SelectedIndex = 3;
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

            bool isStoryBiome = levelName != "GlacierLVL1" && levelName != "JungleLVL1" && levelName != "LavaChamberLVL1" && levelName != "DungeonLVL1" && levelName != "CaveVilageLVL1";

            // Only show story window for the first level of each biome
            if ((levelName.EndsWith("LVL1") && isStoryBiome))
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

        private void MapComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvls_Hidden = false;
            hardcore = false;

            string newImage = string.Empty;

            string currentImage = MapBackground.ImageSource.ToString();

            if (MapComboBox.SelectedIndex == 0) // Earth Map
            {
                newImage = "pack://application:,,,/Images/battleGrounds/map.png";
            }
            else if (MapComboBox.SelectedIndex == 1) // underground Map
            {
                newImage = "pack://application:,,,/Images/battleGrounds/cave.png";
            }
            else if (MapComboBox.SelectedIndex == 2) // Hardcore Earth Map
            {
                newImage = "pack://application:,,,/Images/battleGrounds/marine.png";
            }
            else if (MapComboBox.SelectedIndex == 3) // Space Map
            {
                newImage = "pack://application:,,,/Images/battleGrounds/space.png";
            }

            MapBackground.ImageSource = new BitmapImage(new Uri(newImage));

            Show_Biomes();

            int newCurrentBiomeIndex = currentBiomeIndex - AmountofBiomes;

            if (MapComboBox.SelectedIndex == 0 && newCurrentBiomeIndex >= 0)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else if (MapComboBox.SelectedIndex == 1 && newCurrentBiomeIndex >= 6)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else if (MapComboBox.SelectedIndex == 2 && newCurrentBiomeIndex >= 21)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else if (MapComboBox.SelectedIndex == 3 && newCurrentBiomeIndex >= 29)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else
            {
                HardcoreBtn.Visibility = Visibility.Collapsed;
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

            if (MapComboBox.SelectedIndex == 0)
            {
                string newImage = currentImage.Contains("hardcore_map.png")
                ? "pack://application:,,,/Images/battleGrounds/map.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_map.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            else if (MapComboBox.SelectedIndex == 1)
            {
                string newImage = currentImage.Contains("hardcore_cave.png")
                ? "pack://application:,,,/Images/battleGrounds/cave.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_cave.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            else if (MapComboBox.SelectedIndex == 2)
            {
                string newImage = currentImage.Contains("hardcore_marine.png")
                ? "pack://application:,,,/Images/battleGrounds/marine.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_marine.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            else if (MapComboBox.SelectedIndex == 3)
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

                if (hardcore) { newCurrentBiomeIndex -= AmountofBiomes; }

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
            int newCurrentBiomeIndex = currentBiomeIndex;
            if (hardcore) { newCurrentBiomeIndex -= AmountofBiomes; }

            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                int biomeValue = (int)biome;
                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;



                if (button != null)
                {
                    bool isEarth = (biomeValue <= 5) || (biomeValue >= 14 && biomeValue <= 20) || (biomeValue >= 27 && biomeValue <= 28);

                    if (MapComboBox.SelectedIndex == 0 && isEarth && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else if (MapComboBox.SelectedIndex == 1 && biomeValue >= 6 && biomeValue <= 13 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else if (MapComboBox.SelectedIndex == 2 && biomeValue >= 21 && biomeValue <= 26 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    else if (MapComboBox.SelectedIndex == 3 && biomeValue >= 29 && biomeValue <= 37 && biomeValue <= newCurrentBiomeIndex)
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
            if (!gambleWindow.ShowActivated)
            {
                gambleWindow.Show();
            }
        }
    }
}
