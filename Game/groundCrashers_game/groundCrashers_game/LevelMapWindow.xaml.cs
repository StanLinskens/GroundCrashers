using System.Windows;

namespace groundCrashers_game
{
    /// <summary>
    /// Interaction logic for LevelMapWindow.xaml
    /// </summary>
    public partial class LevelMapWindow : Window
    {
        public LevelMapWindow()
        {
            InitializeComponent();
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
    }
}
