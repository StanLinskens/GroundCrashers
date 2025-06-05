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
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Long ago, the world was balanced by Twelve Elemental Crystals... until chaos crept in.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Aria, an explorer, received a vision—calling her to restore balance. Her journey begins in the Forest.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "This forest feels alive... like it’s watching us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "The Life Crystal is near. But we’re not alone.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "As enchanted animals circle them, Aria solves a twisting tree maze. Rook defends against the pack.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Got it! The Life Crystal is ours!", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
            }
            else if (biomeName == "Cave")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Deep underground, the Cave echoes with danger.", ImagePathPortrait = "", backgroundImage = "Cave" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Watch your step. The Earth Crystal won’t come easy.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Cave" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They dodge falling rocks and deceptive echo-traps.", ImagePathPortrait = "", backgroundImage = "Cave" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "The Earth Crystal... we’ve got it. Let’s move.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Cave" });
            }
            else if (biomeName == "Swamp")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Thick fog and poison air fill the Swamp.", ImagePathPortrait = "", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Can’t see anything... but the fireflies might lead us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Trust nature. It remembers the way.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Using glowing fireflies, they find the Swamp Crystal.", ImagePathPortrait = "", backgroundImage = "Swamp" });
            }else if (biomeName == "Ocean")
            {
            }
            else if (biomeName == "Mountain")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Steep cliffs and roaring wind test their resolve.", ImagePathPortrait = "", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "The Wind Crystal’s peak lies above.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Race you to the top!", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Fighting wind spirits, they reach the summit and claim the Wind Crystal.", ImagePathPortrait = "", backgroundImage = "Mountain" });
            }
            else if (biomeName == "Marsh")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "The Marsh is dense and eerie, spirits lingering in the mist.", ImagePathPortrait = "", backgroundImage = "Marsh" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Every step sinks deeper... how do we go on?", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Marsh" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Trust me, Aria. I won’t let us fall.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Marsh" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "The Spirit Crystal glows as their bond strengthens.", ImagePathPortrait = "", backgroundImage = "Marsh" });
            }
            else if (biomeName == "Tundra")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Frozen winds howl across the Tundra.", ImagePathPortrait = "", backgroundImage = "Tundra" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "The Ice Crystal is close. Stay warm, stay sharp.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Tundra" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They carefully bypass a sleeping frost beast to retrieve the crystal.", ImagePathPortrait = "", backgroundImage = "Tundra" });
            }
            else if (biomeName == "Glacier")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Glittering caverns beneath the Glacier echo with cold silence.", ImagePathPortrait = "", backgroundImage = "Glacier" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Let’s light a fire. Just for a moment.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Glacier" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "With warmth, they find the Cold Crystal nestled in ice.", ImagePathPortrait = "", backgroundImage = "Glacier" });
            }
            else if (biomeName == "Volcano")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Lava flows and heat waves fill the Volcano.", ImagePathPortrait = "", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "We’ll split up. I’ll take the ridge.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I’ll head through the lava tunnels. Stay alive.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Through perilous paths, they secure the Fire Crystal.", ImagePathPortrait = "", backgroundImage = "Volcano" });
            }
            else if (biomeName == "Jungle")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "The Jungle is thick with vines and sounds of unseen creatures.", ImagePathPortrait = "", backgroundImage = "Jungle" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "That serpent’s gaining on us!", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Jungle" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Keep it distracted—I’ll grab the Nature Crystal!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Jungle" });
            }
            else if (biomeName == "Ruin")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Ancient Ruins submerged in sand hide the secrets of time.", ImagePathPortrait = "", backgroundImage = "Ruin" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "This mural... it’s a puzzle.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ruin" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They solve the puzzle and unlock the Time Crystal.", ImagePathPortrait = "", backgroundImage = "Ruin" });
            }
            else if (biomeName == "Crystal Cavern")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Echoes bounce in every direction within the Crystal Cavern.", ImagePathPortrait = "", backgroundImage = "Crystal Cavern" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "We’re going in circles...", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Crystal Cavern" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Wait... the echoes harmonize. Follow the melody.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Crystal Cavern" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "The music leads them to the Echo Crystal.", ImagePathPortrait = "", backgroundImage = "Crystal Cavern" });
            }
            else if (biomeName == "Highlands")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "The sacred Highlands hold Rook’s past.", ImagePathPortrait = "", backgroundImage = "Highlands" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I was once a guardian here... before the fall.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Then it’s time to reclaim your legacy.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Highlands" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Together, they unlock the Light Crystal. The world begins to heal.", ImagePathPortrait = "", backgroundImage = "Highlands" });
            }
            else
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "This biome is still under construction.", ImagePathPortrait = "", backgroundImage = "construction" });
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
