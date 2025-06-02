using System;
using System.Windows;

namespace groundCrashers_game
{
    public partial class StoryWindow : Window
    {
        private string levelName;

        public StoryWindow(string biomeName, string levelName)
        {
            InitializeComponent();
            this.levelName = levelName;
            StoryText.Text = GetStoryForBiome(biomeName);
        }

        private void StartLevel_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow(true, levelName);
            gameWindow.Show();
            this.Close();
            //Application.Current.Windows[0]?.Close(); 
        }

        private string GetStoryForBiome(string biomeName)
        {
            return biomeName switch
            {
                "Forest" => "Welcome to the Forest Biome! Ancient trees whisper secrets of the past.",
                "Desert" => "You’ve entered the Desert Biome. Watch out for hidden dangers beneath the sand.",
                "Mountain" => "High in the Mountain Biome, icy winds test your resolve.",
            };
        }
    }
}
