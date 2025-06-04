using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace groundCrashers_game
{
    public partial class StoryWindow : Window
    {
        private class DialogLine
        {
            public string Character { get; set; }
            public string Text { get; set; }
            public string ImagePathPortrait { get; set; }
            public string backgroundImage { get; set; }
        }

        private List<DialogLine> dialogLines;
        private int currentLine = 0;
        private string levelName;
        private bool hardcore;

        public StoryWindow(string biomeName, string levelName, bool hardcore)
        {
            InitializeComponent();
            this.levelName = levelName;
            this.hardcore = hardcore;

            LoadDialog(biomeName);
            ShowCurrentLine();
        }

        private void LoadDialog(string biomeName)
        {
            dialogLines = new List<DialogLine>();

            if (biomeName == "Forest")
            {
                dialogLines.Add(new DialogLine
                {
                    Character = "Aria",
                    Text = "Welcome to the Forest. The trees whisper tales of long-lost secrets.",
                    ImagePathPortrait = "Images/portraits/aria.png",
                    backgroundImage = "Forest"
                });

                dialogLines.Add(new DialogLine
                {
                    Character = "Rook",
                    Text = "Stay alert, Aria. Something doesn’t feel right...",
                    ImagePathPortrait = "Images/portraits/rook.png",
                    backgroundImage = "Forest"
                });
            }
            else
            {
                dialogLines.Add(new DialogLine
                {
                    Character = "Narrator",
                    Text = "This biome is still under construction.",
                    ImagePathPortrait = "",
                    backgroundImage = ""
                });
            }
        }

        private void ShowCurrentLine()
        {
            if (currentLine >= dialogLines.Count)
            {
                // End of story -> start game
                GameWindow gameWindow = new GameWindow(true, levelName, hardcore);
                gameWindow.Show();
                this.Close();
                return;
            }

            var line = dialogLines[currentLine];

            CharacterName.Text = line.Character;
            DialogText.Text = line.Text;

            // Load character portraits
            try
            {
                CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/aria.png", UriKind.Relative));
                CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/rook.png", UriKind.Relative));
            }
            catch
            {
                // Ignore portrait load failure
            }

            // Load background image if specified
            if (!string.IsNullOrEmpty(line.backgroundImage))
            {
                try
                {
                    var bgUri = new Uri($"pack://application:,,,/Images/battleGrounds/{line.backgroundImage.ToLower()}.jpg");
                    BackGround.ImageSource = new BitmapImage(bgUri);
                }
                catch
                {
                    // Ignore background load failure or set default background
                    BackGround.ImageSource = null;
                }
            }
            else
            {
                BackGround.ImageSource = null; // Clear background if none specified
            }

            // Highlight the speaking character
            if (line.Character == "Aria")
            {
                CharacterImage.Opacity = 1.0;
                CharacterImage2.Opacity = 0.3;
                CharacterImage.Height = 464;
                CharacterImage2.Height = 350;
            }
            else if (line.Character == "Rook")
            {
                CharacterImage.Opacity = 0.3;
                CharacterImage2.Opacity = 1.0;
                CharacterImage2.Height = 464;
                CharacterImage.Height = 350;
            }
            else
            {
                CharacterImage.Opacity = 0.3;
                CharacterImage2.Opacity = 0.3;
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            currentLine++;
            ShowCurrentLine();
        }
    }
}
