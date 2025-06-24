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

        public LevelMapWindow(bool LVLWon = false, string LVLname = "", bool playedHardcore = false)
        {     
            InitializeComponent();

            // hide
            MapComboBox.Visibility = Visibility.Collapsed;
            HardcoreBtn.Visibility = Visibility.Collapsed;

            // get the played level result
            GetPlayedLevelResult(LVLWon, LVLname, playedHardcore);

            // lvl won is false because the level is not won yet
            LVLWon = false;

            // update the account with the current biome and level index
            AccountManager.UpdateActiveAccount();

            // show the biomes dropdown based on the current biome index
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

            // play the map background music
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("map.wav", true);

            // Default to Earth map
            // because of this the MapComboBox_SelectionChanged event triggers
            if(MapComboBox.SelectedIndex == -1) MapComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// to get the result of the played level and update the account accordingly
        /// </summary>
        /// <param name="LVLWon">the lvlwon thing</param>
        /// <param name="LVLname">the name if the level</param>
        /// <param name="playedHardcore">if they played hardcore</param>
        private void GetPlayedLevelResult(bool LVLWon, string LVLname, bool playedHardcore)
        {
            // get the current biome and level index from the active account
            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            // uncomplete biomes
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

                    // if the player played hardcore, the current biome index is reduced by the amount of biomes (hardcore is the same biomes but harder)
                    int newActiveCurrentBiomeIndex = ActiveAccount.Active_current_biome_id;
                    if (playedHardcore) { newActiveCurrentBiomeIndex -= AmountofBiomes; CoinsEarned += 2; }

                    // check if the current biome index is in the range of the current biome
                    completedEarth = ((newActiveCurrentBiomeIndex <= 5) || (newActiveCurrentBiomeIndex >= 14 && newActiveCurrentBiomeIndex <= 20) || (newActiveCurrentBiomeIndex >= 27 && newActiveCurrentBiomeIndex <= 28));
                    completedCave = newActiveCurrentBiomeIndex >= 6 && newActiveCurrentBiomeIndex <= 13;
                    completedMarine = newActiveCurrentBiomeIndex >= 21 && newActiveCurrentBiomeIndex <= 26;
                    completedSpace = newActiveCurrentBiomeIndex >= 29 && newActiveCurrentBiomeIndex <= 37;

                    // if the level is won and the biome and lvl are the same as the current biome and lvl
                    if (LVLWon && biomeValue == newActiveCurrentBiomeIndex && BiomeLVL == currentLVLIndex)
                    {
                        // lvl go up and if lvl is 6, go to next biome
                        ActiveAccount.Active_current_biome_lvl_id++;

                        // check if biome is completed
                        bool completedNewEarth = ActiveAccount.Active_current_biome_lvl_id >= 6 && completedEarth;
                        bool completedNewCave = ActiveAccount.Active_current_biome_lvl_id >= 5 && completedCave;
                        bool completedNewMarine = ActiveAccount.Active_current_biome_lvl_id >= 5 && completedMarine;
                        bool completedNewSpace = ActiveAccount.Active_current_biome_lvl_id >= 4 && completedSpace;

                        // if a biome is completed
                        if (completedNewEarth || completedNewMarine || completedNewSpace || completedNewCave)
                        {
                            // reset the lvl to 1
                            ActiveAccount.Active_current_biome_lvl_id = 1;
                            ActiveAccount.Active_current_biome_id++; // biome goes up
                            xpEarned *= 2; // Extra XP for completing a biome
                        }

                        // these levels have a story window that needs to be shown
                        bool isLastBiomeStory = LVLname == "CrystalCavernLVL5" || LVLname == "AltarLVL4" || LVLname == "OceanLVL5" || LVLname == "HydroVentLVL4" || LVLname == "VolcanoLVL5";
                        if (isLastBiomeStory)
                        {
                            // show the story window for the last level of each biome
                            StoryWindow storyWindow = new StoryWindow(LVLname, LVLname, playedHardcore);
                            storyWindow.ShowDialog(); // <- this blocks the current thread until the window is closed
                        }
                    }
                    // update the players XP
                    ActiveAccount.Active_XP += xpEarned;
                    AccountManager.LevelUp();

                    ActiveAccount.Active_coins += CoinsEarned; // Add coins for winning the level
                }
            }
            // update the current biome and level index in the active account
            AccountLVLVisualChage();

            // save the current biome and level index
            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            // display the map based on the completed biomes
            if (completedEarth) MapComboBox.SelectedIndex = 0;
            else if (completedCave) MapComboBox.SelectedIndex = 1;
            else if (completedMarine) MapComboBox.SelectedIndex = 2;
            else if (completedSpace) MapComboBox.SelectedIndex = 3;
        }

        /// <summary>
        /// leave the level map window and go back to the main window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow MainWindow = new MainWindow();
            MainWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Handles the click event for the "Display Map" button, toggling the visibility of biome buttons.
        /// </summary>

        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayMapButton_Click(object sender, RoutedEventArgs e)
        {
            // lvls are now chagned arround
            // if it was hidden they show and vice versa
            lvls_Hidden = !lvls_Hidden;

            // get the current image source of the map background
            string currentImage = MapBackground.ImageSource.ToString();
            // remove the pack://application:,,,/Images/battleGrounds/ part from the image source
            string Image = currentImage.Replace("pack://application:,,,/Images/battleGrounds/", "");

            // if the levels are hidden
            if (lvls_Hidden)
            {
                // go trough all biomes and hide the buttons
                foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
                {
                    // make it a int
                    int biomeValue = (int)biome;

                    // make it a button
                    var buttonName = $"{biome}Button";
                    var button = this.FindName(buttonName) as UIElement;

                    // if the button is not null and the biome is not the current biome, hide it
                    if (button != null)
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }
            }
            // if it is not hidden
            else
            {
                // show the biomes
                Show_Biomes();
            }   
        }

        /// <summary>
        /// button click event handler for starting a level.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            // get the button that was clicked
            Button clicked = sender as Button;
            string levelName = clicked.Name;

            // Get biome name from levelName (e.g., ForestLVL1 -> Forest)
            string biomeName = levelName.Replace("LVL1", "").Replace("Button", "");

            // Split the level name to get the biome name (e.g., "GlacierLVL1" -> "Glacier")
            string[] biomeNameAll = levelName.Split("LVL");

            // check if the level is a hardcore level
            string currentImage = MapBackground.ImageSource.ToString();
            bool isHardcore = currentImage.Contains("hardcore");

            // these are all not story biomes
            bool isStoryBiome = levelName != "GlacierLVL1" && levelName != "JungleLVL1" && levelName != "LavaChamberLVL1" && levelName != "DungeonLVL1" && levelName != "CaveVilageLVL1" && levelName != "SwampLVL1" &&
                                levelName != "MarshLVL1" && levelName != "TundraLVL1" && levelName != "RuinsLVL1" && levelName != "CoralReefLVL1" && levelName != "OpenOceanLVL1" && levelName != "ColdSeepLVL1" &&
                                levelName != "NebulaLVL1" && levelName != "InterstellarLVL1" && levelName != "DebrisLVL1" && levelName != "SaturnLVL1" && levelName != "CybertronLVL1" && levelName != "AsteroidsLVL1";

            // check if the level is already pleayed
            bool isntPlayedYet = IsStoryPlayed(biomeNameAll[0]);

            // check if the level is a sun level
            bool isSunLvl = levelName == "SunLVL2" || levelName == "SunLVL3";

            // Only show story window for the first level of each biome
            if ((levelName.EndsWith("LVL1") && isStoryBiome && isntPlayedYet) || isSunLvl )
            {
                // show story levels
                StoryWindow storyWindow = new StoryWindow(biomeName, levelName, isHardcore);
                storyWindow.Show();
                this.Close();
            }
            // if the level is not a story level, start the game directly
            else
            {
                // start the game directly
                GameWindow gameWindow = new GameWindow(true, levelName, isHardcore);
                gameWindow.Show();
                this.Close();
            }
        }

        /// <summary>
        /// to check if the story for a specific biome has been played
        /// </summary>
        /// <param name="biomeName"></param>
        /// <returns></returns>
        private bool IsStoryPlayed(string biomeName)
        {
            // get the current biome index from the active account
            int newCurrentBiomeIndex = currentBiomeIndex;
            if (hardcore) newCurrentBiomeIndex -= AmountofBiomes;

            // check if the current biome index is the same as the biome name
            if (newCurrentBiomeIndex == (int)Enum.Parse(typeof(Biomes), biomeName) && ActiveAccount.Active_current_biome_lvl_id == 1) return true;
            return false; // means not seen yet so it playes the story
        }

        /// <summary>
        /// map combobox selection changed event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MapComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // hide the levels
            lvls_Hidden = false;
            hardcore = false;

            // reset the current biome index and level index
            string newImage = string.Empty;
            string currentImage = MapBackground.ImageSource.ToString();
            int newCurrentBiomeIndex = currentBiomeIndex - AmountofBiomes;

            if (MapComboBox.SelectedIndex == 0) // Earth Map
            {
                if (newCurrentBiomeIndex >= 0 && newCurrentBiomeIndex <= 28) { newImage = "pack://application:,,,/Images/battleGrounds/hardcore_map.png"; hardcore = true; } 
                else {newImage = "pack://application:,,,/Images/battleGrounds/map.png"; hardcore = false; } 
            }
            else if (MapComboBox.SelectedIndex == 1) // underground Map
            {
                if (newCurrentBiomeIndex >= 6 && newCurrentBiomeIndex <= 13) { newImage = "pack://application:,,,/Images/battleGrounds/hardcore_cave.png"; hardcore = true; }
                else { newImage = "pack://application:,,,/Images/battleGrounds/cave.png"; hardcore = false; }
            }
            else if (MapComboBox.SelectedIndex == 2) // Hardcore Earth Map
            {
                if (newCurrentBiomeIndex >= 21 && newCurrentBiomeIndex <= 26) { newImage = "pack://application:,,,/Images/battleGrounds/hardcore_marine.png"; hardcore = true; }
                else { newImage = "pack://application:,,,/Images/battleGrounds/marine.png"; hardcore = false; }
            }
            else if (MapComboBox.SelectedIndex == 3) // Space Map
            {
                if (newCurrentBiomeIndex >= 29 && newCurrentBiomeIndex <= 37) { newImage = "pack://application:,,,/Images/battleGrounds/hardcore_space.png"; hardcore = true; }
                else { newImage = "pack://application:,,,/Images/battleGrounds/space.png"; hardcore = false; }
            }

            // get img
            MapBackground.ImageSource = new BitmapImage(new Uri(newImage));

            // display the levels based on the new biome index
            Display_Levels();

            // display the current biome index and level index
            Show_Biomes();

            // if all the levels have been completed, show the hardcore button to enable/disable hardcore mode
            if (newCurrentBiomeIndex >= 38)
            {
                HardcoreBtn.Visibility = Visibility.Visible;
            }
            else // hide the hardcore button if the levels are not completed
            {
                HardcoreBtn.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// if the biome button is clicked, toggle the popup for that biome.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BiomeBtn_Click(object sender, RoutedEventArgs e)
        {
            // get button info
            Button clicked = sender as Button;
            string buttonFullName = clicked.Name;
            string biomeName = buttonFullName.Replace("Button", "");

            // check if the biome name is valid
            if (biomeName != null)
            {
                // show the popup for the clicked biome
                var popupName = $"{biomeName}Popup";
                var popup = this.FindName(popupName) as Popup;
                if (popup != null)
                {
                    popup.IsOpen = !popup.IsOpen;
                }
            }
        }

        /// <summary>
        /// if hardcore button is clicked, toggle hardcore mode and change the map background image accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HardcoreBtn_Click(object sender, RoutedEventArgs e)
        {
            // toggle hardcore mode
            hardcore = !hardcore;
            lvls_Hidden = false;

            // get the current image source of the map background
            string currentImage = MapBackground.ImageSource.ToString();

            // if earth
            if (MapComboBox.SelectedIndex == 0)
            {
                // if current image contains hardcore map, switch to normal map and vice versa
                string newImage = currentImage.Contains("hardcore_map.png")
                ? "pack://application:,,,/Images/battleGrounds/map.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_map.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            // if cave
            else if (MapComboBox.SelectedIndex == 1)
            {
                // if current image contains hardcore cave, switch to normal cave and vice versa
                string newImage = currentImage.Contains("hardcore_cave.png")
                ? "pack://application:,,,/Images/battleGrounds/cave.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_cave.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            // if marine
            else if (MapComboBox.SelectedIndex == 2)
            {
                // if current image contains hardcore marine, switch to normal marine and vice versa
                string newImage = currentImage.Contains("hardcore_marine.png")
                ? "pack://application:,,,/Images/battleGrounds/marine.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_marine.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }
            // if space
            else if (MapComboBox.SelectedIndex == 3)
            {
                // if current image contains hardcore space, switch to normal space and vice versa
                string newImage = currentImage.Contains("hardcore_space.png")
                ? "pack://application:,,,/Images/battleGrounds/space.png"
                : "pack://application:,,,/Images/battleGrounds/hardcore_space.png";

                MapBackground.ImageSource = new BitmapImage(new Uri(newImage));
            }

            Display_Levels();
            Show_Biomes();
        }

        /// <summary>
        /// display_Levels hides or shows the levels based on the current biome index and level index.
        /// </summary>
        private void Display_Levels()
        {
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                // get the current biome index from the active account
                int newCurrentBiomeIndex = currentBiomeIndex;

                // if hardcore is enabled, reduce the current biome index by the amount of biomes
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

        /// <summary>
        /// this shows the biomes based on the current biome index and the selected map in the combobox
        /// </summary>
        private void Show_Biomes()
        {
            // if the levels are hidden, show them
            int newCurrentBiomeIndex = currentBiomeIndex;
            if (hardcore) { newCurrentBiomeIndex -= AmountofBiomes; }

            // go trough all biomes and show the buttons based on the current biome index and the selected map in the combobox
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                int biomeValue = (int)biome;
                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;

                // check if the biome button is not null
                if (button != null)
                {
                    // to see what is earth
                    bool isEarth = (biomeValue <= 5) || (biomeValue >= 14 && biomeValue <= 20) || (biomeValue >= 27 && biomeValue <= 28);

                    // if it is earth show only earth
                    if (MapComboBox.SelectedIndex == 0 && isEarth && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    // if it is cave show only cave
                    else if (MapComboBox.SelectedIndex == 1 && biomeValue >= 6 && biomeValue <= 13 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    // if it is marine show only marine
                    else if (MapComboBox.SelectedIndex == 2 && biomeValue >= 21 && biomeValue <= 26 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    // if it is space show only space
                    else if (MapComboBox.SelectedIndex == 3 && biomeValue >= 29 && biomeValue <= 37 && biomeValue <= newCurrentBiomeIndex)
                    {
                        button.Visibility = Visibility.Visible;
                    }
                    // dont show it if it doesnt exist
                    else
                    {
                        button.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// to go to the gamble window when the capsule button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CapsuleBtn_Click(object sender, RoutedEventArgs e)
        {
            GambleWindow gambleWindow = new GambleWindow(this);
            gambleWindow.ShowDialog();
            
        }

        /// <summary>
        /// accountLVLVisualChage updates the current level and XP text in the UI
        /// </summary>
        public void AccountLVLVisualChage()
        {
            currentLVL.Text = $"LVL: {ActiveAccount.Active_LVL}"; // Update the current level text
            currentXP.Text = $"XP: {ActiveAccount.Active_XP}"; // Update the current XP text
        }
    }
}
