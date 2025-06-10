using DocumentFormat.OpenXml.Office2016.Drawing.Command;
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
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There once was a tale of five Chaos Orbs, ancient relics that normally protect these lands.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "But Aria received a vision—a warning that the Orbs would shatter, and chaos would seep into the world.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "She confided in Rook, and together they set out to prevent the looming disaster.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "This forest feels... alive. Like it's watching us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "We need to move quickly. I don’t know how much time we have left.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Wait... Do you see that?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethings comming from behind those trees.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "there it was a GroundCrasher. luckily Aria was prepared.", ImagePathPortrait = "", backgroundImage = "Forest" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Get behind me Rook! i will handel this.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
            }
            else if (biomeName == "Wasteland")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria, do you have any idea where we’re going?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Wasteland" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Not exactly. But we have to keep moving—and I have a feeling there’s something inside that cavern.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Wasteland" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "What they didn’t know was that in the three biomes ahead, more GroundCrashers awaited.", ImagePathPortrait = "", backgroundImage = "Wasteland" });
            }
            else if (biomeName == "Mountain")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Could it be that your GroundCrasher was weaker in the other biome? i have a feeling that health was lower.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "i only know that GroundCrashers primary-type's have other types they are strong/weak against. not that biomes do debuffs.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "to find out more information abount GroundCrashers go to: https://stan.1pc.nl/GroundCrashers/Website/index.html", ImagePathPortrait = "", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "i also know that sommethimes you need multiple GroundCrashers so it might be time to go to the gamble store for more groundcrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the gamble store can be found by pressing the button in the left top of the map.", ImagePathPortrait = "", backgroundImage = "Mountain" });
            }
            else if (biomeName == "CrystalCavern")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Were finaly in the cavern. i know there has to be something here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets get rid of these GroundCrashers so we can look in peace.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
            }
            else if (biomeName == "CrystalCavernLVL5")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They have been looking for hours but coudnt find anything..", ImagePathPortrait = "", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria are you sure this feeling you have means something? because i cant find anything.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "i am starting to doubt myself two. lets leave", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "AAAAAAHHH", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria are you alright?! I will come down.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "they both went down a hidden trapdoor. heading deeper into the cave", ImagePathPortrait = "", backgroundImage = "cave" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the cave map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "cave" });
            }
            else if (biomeName == "Catacomb")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I knew I was right. I know it is here, lets go", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Calm down first, you took a hard fall.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Catacomb" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "There is no time for that we have to fight.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
            }
            else if (biomeName == "CaveCitadel")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I found something. It says:", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I am at a place that blankets the Earth, both deep and wide.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "With secrets and creatures I always hide.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "It crashes, It roares, yet sometimes It is still.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "With tides that move at nature's will.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think it has to mean something. but lets fight these GroundCrashers first.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveCitadel" });
            }
            else if (biomeName == "CaveLake")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Could the awnser be lake?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveLake" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I don't Know. lets search this place just in case.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveLake" });
            }
            else if (biomeName == "FungalHollow")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "It has to mean something else, we have been searching this place for houres now.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "We arnt even at the lake anymore.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Calm down. We can go back to the lake if the chaos orb isnt in the rest of the cave", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "FungalHollow" });
            }
            else if (biomeName == "Altar")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There it is the first chaos orb.", ImagePathPortrait = "", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets get it. do you know how to keep it save.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "No i dont. you defend against the GroundCrashers. i will figure out what to do with it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
            }
            else if (biomeName == "AltarLVL4")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I figured it out. i know how to encage it", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Let me work my magic.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Ok i will get out of the way.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrb.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged1.png", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged.png", backgroundImage = "Altar" });
            }
            else if (biomeName == "Swamp")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Thick fog and poison air fill the Swamp.", ImagePathPortrait = "", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Can’t see anything... but the fireflies might lead us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Trust nature. It remembers the way.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Swamp" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Using glowing fireflies, they find the Swamp Crystal.", ImagePathPortrait = "", backgroundImage = "Swamp" });
            }
            else if (biomeName == "Ocean")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "Beneath the waves, the Ocean holds secrets of the deep.", ImagePathPortrait = "", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "The Water Crystal is guarded by ancient sea spirits.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Let’s swim deeper. I can feel its pull.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They navigate coral mazes and outsmart a giant squid to claim the Water Crystal.", ImagePathPortrait = "", backgroundImage = "Ocean" });
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
            bool isLastBiomeStory = levelName == "CrystalCavernLVL5" || levelName == "AltarLVL4";

            if (currentLine >= dialogLines.Count && !isLastBiomeStory)
            {
                // End of story -> start game
                GameWindow gameWindow = new GameWindow(true, levelName, hardcore);
                gameWindow.Show();
                this.Close();
                return;
            }
            else if (currentLine >= dialogLines.Count && isLastBiomeStory)
            {
                this.Close();
                return; // Close the story window if it's the last line in the last biome story
            }

                var line = dialogLines[currentLine];

            CharacterName.Text = line.Character;
            DialogText.Text = line.Text;

            // Load character portraits
            try
            {
                CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/aria.png", UriKind.Relative));
                if (line.ImagePathPortrait == "Images/portraits/ChaosOrb.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrb.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged1.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged1.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged.png", UriKind.Relative)); }
                else { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/rook.png", UriKind.Relative)); }

            }
            catch
            {
                // Ignore portrait load failure
            }

            // Load background image if specified
            if (!string.IsNullOrEmpty(line.backgroundImage))
            {
                bool isPng = line.backgroundImage == "cave";
                try
                {
                    if (!isPng) { var bgUri = new Uri($"pack://application:,,,/Images/battleGrounds/{line.backgroundImage.ToLower()}.jpg"); BackGround.ImageSource = new BitmapImage(bgUri); }
                    else { var bgUri = new Uri($"pack://application:,,,/Images/battleGrounds/{line.backgroundImage.ToLower()}.png"); BackGround.ImageSource = new BitmapImage(bgUri); }
                    
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
            else if (line.Character == "ChaosOrb")
            {
                CharacterImage.Opacity = 1.0;
                CharacterImage2.Opacity = 1.0;
                CharacterImage.Height = 464;
                CharacterImage2.Height = 464;
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
