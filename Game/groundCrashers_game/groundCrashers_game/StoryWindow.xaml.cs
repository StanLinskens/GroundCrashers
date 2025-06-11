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
        private bool ariaDead = false; // Track if Aria is dead

        public StoryWindow(string biomeName, string levelName, bool hardcore)
        {
            InitializeComponent();
            this.levelName = levelName;
            this.hardcore = hardcore;

            if (this.hardcore)
            {
                ariaDead = true;
            }

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
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "and so the first", ImagePathPortrait = "", backgroundImage = "Altar" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "m¡$†@k€.", ImagePathPortrait = "", backgroundImage = "hardcore_cave" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "was made.", ImagePathPortrait = "", backgroundImage = "Altar" });
            }
            else if (biomeName == "Highlands")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Could the awnser be ocean?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "To the riddle.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "It does make sence. we can search there when we get to it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Highlands" });
            }
            else if (biomeName == "Savanna")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethimes these fights are just unfair.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Savanna" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "yeah they get 4 and we only 3.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "We have to level up. we cant lose here the whole world is in danger.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
            }
            else if (biomeName == "Ocean")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "If we are right about the awnser of the riddle. than it should be here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "It has to be right? This awnser makes the most sense.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets look at the surface first. Than we can go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
            }
            else if (biomeName == "OceanLVL5")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I think i see something down there. it has to be the orb.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "It has to be.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the marine map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "marine" });
            }
            else if (biomeName == "Estuaries")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Look how big it is down here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "How are we able to breath under water?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Estuaries" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
            }
            else if (biomeName == "DeepCoralReef")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "The GroundCrashers are getting alot stronger.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I also think they change if we figth the same lvl again.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "idk it could be the case.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "DeepCoralReef" });
            }
            else if (biomeName == "HydroVent")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the second chaos orb.", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets go fast.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "First lets fight these GroundCrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
            }
            else if (biomeName == "HydroVentLVL4")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I will encage it again. get back", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrb.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged1.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "this one wont spread chaos anymore.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "He didnt know how", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "₩ЯØП₲", ImagePathPortrait = "", backgroundImage = "hardcore_marine" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "he was", ImagePathPortrait = "", backgroundImage = "HydroVent" });
            }
            else if (biomeName == "Desert")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "The last one has to be in the vulcano right?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Desert" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "I think so two. we have to go quick. what if it is more unsteable because of the heat", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Desert" });
            }
            else if (biomeName == "Volcano")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Is that a rocket?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "What if there is an orb in space?", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "It could be. lets first search the vulcano. Maybe there is one here two", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
            }
            else if (biomeName == "VolcanoLVL5")
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the third orb", ImagePathPortrait = "", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "there is an orb.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Encage it quicly. what if it breaks right now?!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrb.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged1.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged.png", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "He was right. this one ¡$ going to", ImagePathPortrait = "", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "฿®£∆k", ImagePathPortrait = "", backgroundImage = "hardcore_map" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "any minute.", ImagePathPortrait = "", backgroundImage = "Volcano" });
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the space map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "space" });
            }
            else if (biomeName == "Earth")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I always wanted to go to space. now i am finaly here", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Earth" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "How are we able to breath in space??", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Earth" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice", ImagePathPortrait = "Images/portraits/Aria.png", backgroundImage = "Earth" });
            }
            else if (biomeName == "Moon")
            {
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "What is a titan?! they are so strong.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Moon" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "yeah, but we need to fight them.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Moon" });
            }
            else if (biomeName == "Sun")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "He has to be here. He has to.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Who do you mean?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "He will be so proud of me.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "What do you mean?!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
            }
            else if (biomeName == "SunLVL2")
            {
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see him.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Who do you mean? Please tell me.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "I have to go.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Dont leave me here With these titans!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
            }
            else if (biomeName == "SunLVL3")
            {
                dialogLines.Add(new DialogLine { Character = "God", Text = "Good job my servent Aria. you did well", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "God", Text = "But you have no use for me now", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                ariaDead = true; // Aria dies in this part of the story
                dialogLines.Add(new DialogLine { Character = "Aria", Text = "Urgh…", ImagePathPortrait = "Images/portraits/ariadead.png", backgroundImage = "Sun" });
                dialogLines.Add(new DialogLine { Character = "Rook", Text = "Noooooooo.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
            }
            else
            {
                dialogLines.Add(new DialogLine { Character = "Narrator", Text = "This biome is still under construction.", ImagePathPortrait = "", backgroundImage = "construction" });
            }
        }


        private void ShowCurrentLine()
        {
            bool isLastBiomeStory = levelName == "CrystalCavernLVL5" || levelName == "AltarLVL4" || levelName == "OceanLVL5" || levelName == "HydroVentLVL4" || levelName == "VolcanoLVL5";

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
                if(ariaDead) CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/ariadead.png", UriKind.Relative));
                else CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/aria.png", UriKind.Relative));

                if (line.ImagePathPortrait == "Images/portraits/ChaosOrb.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrb.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged1.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged1.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/GroundCrasherSprites/pattje72.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/GroundCrasherSprites/pattje72.png", UriKind.Relative)); }
                else { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/rook.png", UriKind.Relative)); }

            }
            catch
            {
                // Ignore portrait load failure
            }

            // Load background image if specified
            if (!string.IsNullOrEmpty(line.backgroundImage))
            {
                bool isPng = line.backgroundImage == "cave" || line.backgroundImage == "hardcore_cave" || line.backgroundImage == "marine" || line.backgroundImage == "hardcore_marine" || 
                             line.backgroundImage == "hardcore_map" || line.backgroundImage == "space";
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
            else if (line.Character == "Rook" || line.Character == "God")
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
