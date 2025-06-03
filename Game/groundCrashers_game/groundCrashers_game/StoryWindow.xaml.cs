using System;
using System.Windows;

namespace groundCrashers_game
{
    public partial class StoryWindow : Window
    {
        private string levelName;
        private bool hardcore;

        public StoryWindow(string biomeName, string levelName, bool hardcore)
        {
            InitializeComponent();
            this.levelName = levelName;
            this.hardcore = hardcore;
            try
            {
            StoryText.Text = GetStoryForBiome(biomeName);
            } catch
            {
                return;
            }
        }

        private void StartLevel_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(true, levelName, hardcore);
            gameWindow.Show();
            this.Close();
            //Application.Current.Windows[0]?.Close(); 
        }

        private string GetStoryForBiome(string biomeName)
        {
            try
            {
                return biomeName switch
                {
                    "Forest" => "Welcome to the Forest Biome! Ancient trees whisper secrets of the past.",
                    "Desert" => "You’ve entered the Desert Biome. Watch out for hidden dangers beneath the sand.",
                    "Mountain" => "High in the Mountain Biome, icy winds test your resolve.",
                };
            }
            catch (Exception ex)
            {
                return "FUCK YOU";
            }

        }
    }
}
