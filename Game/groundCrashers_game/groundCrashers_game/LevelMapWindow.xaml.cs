using DocumentFormat.OpenXml.Drawing;
using groundCrashers_game.classes;
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

        public bool LVLWon { get; set; } = false;
        public int currentBiomeIndex { get; set; } = 0;
        public int currentLVLIndex { get; set; } = 1;

        bool lvls_Hidden = true;

        bool spaceMap_hidden = true;

        public LevelMapWindow(bool LVLWon = false)
        {
            InitializeComponent();

            SecretSpaceBtn.Visibility = Visibility.Collapsed;

            if (LVLWon)
            {
                ActiveAccount.Active_current_biome_lvl_id++;
                if (ActiveAccount.Active_current_biome_lvl_id == 6)
                {
                    ActiveAccount.Active_current_biome_lvl_id = 1;
                    ActiveAccount.Active_current_biome_id++;
                }
            }

            currentBiomeIndex = ActiveAccount.Active_current_biome_id;
            currentLVLIndex = ActiveAccount.Active_current_biome_lvl_id;

            LVLWon = false;

            AccountManager.UpdateActiveAccount();

            if (currentBiomeIndex >= 16)
            {
                SecretSpaceBtn.Visibility = Visibility.Visible;
            }

            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.PlaySpecific("map.wav",true);

            Show_Levels();
        }

        private void Show_Levels()
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
                    buttonBiome.Visibility = Visibility.Visible;

                    // hide all levels for this biome exept the first one
                    for (int i = 1; i <= 5; i++)
                    {
                        var buttonLVLName = $"{biome}LVL{i}";
                        var buttonLVL = this.FindName(buttonLVLName) as Button;
                        if (buttonLVL != null)
                        {
                            buttonLVL.Visibility = Visibility.Visible;
                        }
                    }
                }
                // all lvl above the current biome index are hidden
                if (biomeValue > currentBiomeIndex || biomeValue >= 16)
                {
                    // get the button name from the enum value
                    buttonBiomeName = $"{biome}Button";
                    buttonBiome = this.FindName(buttonBiomeName) as Button;
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
                    }
                }
                // if the biome is the current biome, show all levels up to the current level index
                else if (biomeValue == currentBiomeIndex)
                {
                    // all levels after the current lvl are hidden
                    for (int i = 5; i > currentLVLIndex; i--)
                    {
                        var buttonLVLName = $"{biome}LVL{i}";
                        var buttonLVL = this.FindName(buttonLVLName) as Button;
                        if (buttonLVL != null)
                        {
                            buttonLVL.Visibility = Visibility.Collapsed;
                        }
                    }
                    // all levels below the current level index are green
                    for (int i = 1; i < currentLVLIndex; i++)
                    {
                        var buttonLVLName = $"{biome}LVL{i}";
                        var buttonLVL = this.FindName(buttonLVLName) as Button;
                        if (buttonLVL != null)
                        {
                            buttonLVL.Background = Brushes.Green;
                            buttonLVL.BorderBrush = Brushes.LightGreen;
                        }
                    }
                }
                // if the biome is below the current biome, show all levels and make them greens
                else
                {
                    for (int i = 0; i <= 5; i++)
                    {
                        var buttonLVLName = $"{biome}LVL{i}";
                        var buttonLVL = this.FindName(buttonLVLName) as Control;

                        if (buttonLVL != null)
                        {
                            buttonLVL.Background = Brushes.Green;
                            buttonLVL.BorderBrush = Brushes.LightGreen;
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
            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                int biomeValue = (int)biome;

                var buttonName = $"{biome}Button";
                var button = this.FindName(buttonName) as UIElement;
                if (button != null)
                {
                    if (spaceMap_hidden && biomeValue <= 15)
                    {
                        if (!lvls_Hidden && biomeValue <= currentBiomeIndex) button.Visibility = Visibility.Visible;
                        else button.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        if (!lvls_Hidden && !spaceMap_hidden && biomeValue >= 16 && biomeValue <= currentBiomeIndex) button.Visibility = Visibility.Visible;
                        else button.Visibility = Visibility.Collapsed;
                    }

                }
            }
            lvls_Hidden = !lvls_Hidden;
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

        private void SecretSpaceBtn_Click(object sender, RoutedEventArgs e)
        {
            spaceMap_hidden = !spaceMap_hidden;

            string currentImage = MapBackground.ImageSource.ToString();

            string newImage = currentImage.Contains("space.png")
                ? "pack://application:,,,/Images/battleGrounds/map.png"
                : "pack://application:,,,/Images/battleGrounds/space.png";

            MapBackground.ImageSource = new BitmapImage(new Uri(newImage));

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
                    else if (!spaceMap_hidden && biomeValue >= 16 && biomeValue <= currentBiomeIndex)
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
    }
}
