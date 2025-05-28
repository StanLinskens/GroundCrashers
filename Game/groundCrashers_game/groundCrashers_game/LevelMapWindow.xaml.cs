using DocumentFormat.OpenXml.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for LevelMapWindow.xaml
    /// </summary>
    public partial class LevelMapWindow : Window
    {
        // to get current biome and level index
        public int currentBiomeIndex { get; set; } = 15;
        public int currentLVLIndex { get; set; } = 5;

        bool lvls_Hidden = false;

        public LevelMapWindow()
        {
            InitializeComponent();

            foreach (Biomes biome in Enum.GetValues(typeof(Biomes)))
            {
                // make the enum value an int
                int biomeValue = (int)biome;
                // all lvl above the current biome index are hidden
                if (biomeValue > currentBiomeIndex)
                {
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
                    }
                }
                // if the biome is the current biome, show all levels up to the current level index
                else if (biomeValue == currentBiomeIndex)
                {
                    // all levels after the current biome are hidden
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

        // Forest Biome
        private void ForestChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            ForestPopup.IsOpen = !ForestPopup.IsOpen;
        }

        // Desert Biome
        private void DesertChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            DesertPopup.IsOpen = !DesertPopup.IsOpen;
        }

        // Mountain Biome
        private void MountainChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            MountainPopup.IsOpen = !MountainPopup.IsOpen;
        }

        // Highlands Biome
        private void HighlandsChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            HighlandsPopup.IsOpen = !HighlandsPopup.IsOpen;
        }

        // Glacier Biome
        private void GlacierChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            GlacierPopup.IsOpen = !GlacierPopup.IsOpen;
        }

        // Wasteland Biome
        private void WastelandChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            WastelandPopup.IsOpen = !WastelandPopup.IsOpen;
        }

        // Swamp Biome
        private void SwampChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            SwampPopup.IsOpen = !SwampPopup.IsOpen;
        }

        // Ocean Biome
        private void OceanChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            OceanPopup.IsOpen = !OceanPopup.IsOpen;
        }

        // Volcano Biome
        private void VolcanoChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            VolcanoPopup.IsOpen = !VolcanoPopup.IsOpen;
        }

        // Savanna Biome
        private void SavannaChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            SavannaPopup.IsOpen = !SavannaPopup.IsOpen;
        }

        // Jungle Biome
        private void JungleChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            JunglePopup.IsOpen = !JunglePopup.IsOpen;
        }

        // Tundra Biome
        private void TundraChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            TundraPopup.IsOpen = !TundraPopup.IsOpen;
        }

        // Marsh Biome
        private void MarshChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            MarshPopup.IsOpen = !MarshPopup.IsOpen;
        }

        // Cave Biome
        private void CaveChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            CavePopup.IsOpen = !CavePopup.IsOpen;
        }

        // Ruins Biome
        private void RuinsChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            RuinsPopup.IsOpen = !RuinsPopup.IsOpen;
        }

        // Crystal Cavern Biome
        private void CrystalCavernChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            CrystalCavernPopup.IsOpen = !CrystalCavernPopup.IsOpen;
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
                    if (lvls_Hidden && biomeValue <= currentBiomeIndex) button.Visibility = Visibility.Visible;
                    else button.Visibility = Visibility.Collapsed;
                }
            }
            lvls_Hidden = !lvls_Hidden;
        }
    }
}
