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
        private bool ariaKilled = false;

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

            if(!hardcore)
            {
                switch (biomeName)
                {
                    case "Forest":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There once was a tale of five Chaos Orbs, ancient relics that normally protect these lands.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "But Aria received a vision—a warning that the Orbs would shatter, and chaos would seep into the world.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "She confided in Rook, and together they set out to prevent the looming disaster.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "This forest feels... alive. Like it's watching us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "We need to move quickly. I don’t know how much time we have left.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Wait... Do you see that?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethings comming from behind those trees.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "there it was a GroundCrasher. luckily Aria was prepared.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Get behind me Rook! i will handel this.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        break;
                    case "Wasteland":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria, do you have any idea where we’re going?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Wasteland" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Not exactly. But we have to keep moving—and I have a feeling there’s something inside that cavern.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Wasteland" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "What they didn’t know was that in the three biomes ahead, more GroundCrashers awaited.", ImagePathPortrait = "", backgroundImage = "Wasteland" });
                        break;
                    case "Mountain":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Could it be that your GroundCrasher was weaker in the other biome? i have a feeling that health was lower.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i only know that GroundCrashers primary-type's have other types they are strong/weak against. not that biomes do debuffs.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "to find out more information abount GroundCrashers go to: https://stan.1pc.nl/GroundCrashers/Website/index.html", ImagePathPortrait = "", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i also know that sommethimes you need multiple GroundCrashers so it might be time to go to the gamble store for more groundcrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the gamble store can be found by pressing the button in the left top of the map.", ImagePathPortrait = "", backgroundImage = "Mountain" });
                        break;
                    case "CrystalCavern":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Were finaly in the cavern. i know there has to be something here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets get rid of these GroundCrashers so we can look in peace.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        break;
                    case "CrystalCavernLVL5":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They have been looking for hours but coudnt find anything..", ImagePathPortrait = "", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria are you sure this feeling you have means something? because i cant find anything.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i am starting to doubt myself two. lets leave", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "AAAAAAHHH", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria are you alright?! I will come down.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "they both went down a hidden trapdoor. heading deeper into the cave", ImagePathPortrait = "", backgroundImage = "cave" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the cave map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "cave" });
                        break;
                    case "Catacomb":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I knew I was right. I know it is here, lets go", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Calm down first, you took a hard fall.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Catacomb" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "There is no time for that we have to fight.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
                        break;
                    case "CaveCitadel":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I found something. It says:", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I am at a place that blankets the Earth, both deep and wide.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "With secrets and creatures I always hide.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "It crashes, It roares, yet sometimes It is still.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "With tides that move at nature's will.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think it has to mean something. but lets fight these GroundCrashers first.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveCitadel" });
                        break;
                    case "CaveLake":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Could the awnser be lake?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveLake" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I don't Know. lets search this place just in case.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveLake" });
                        break;
                    case "FungalHollow":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It has to mean something else, we have been searching this place for houres now.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "We arnt even at the lake anymore.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Calm down. We can go back to the lake if the chaos orb isnt in the rest of the cave", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "FungalHollow" });
                        break;
                    case "Altar":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There it is the first chaos orb.", ImagePathPortrait = "", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets get it. do you know how to keep it save.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "No i dont. you defend against the GroundCrashers. i will figure out what to do with it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        break;
                    case "AltarLVL4":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I figured it out. i know how to encage it", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Let me work my magic.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Ok i will get out of the way.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrb.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged1.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "and so the first", ImagePathPortrait = "", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "m¡$†@k€.", ImagePathPortrait = "", backgroundImage = "hardcore_cave" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "was made.", ImagePathPortrait = "", backgroundImage = "Altar" });
                        break;
                    case "Highlands":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Could the awnser be ocean?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "To the riddle.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It does make sence. we can search there when we get to it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Highlands" });
                        break;
                    case "Savanna":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethimes these fights are just unfair.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Savanna" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "yeah they get 4 and we only 3.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "We have to level up. we cant lose here the whole world is in danger.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
                        break;
                    case "Ocean":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "If we are right about the awnser of the riddle. than it should be here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "It has to be right? This awnser makes the most sense.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets look at the surface first. Than we can go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        break;
                    case "OceanLVL5":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I think i see something down there. it has to be the orb.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "It has to be.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the marine map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "marine" });
                        break;
                    case "Estuaries":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Look how big it is down here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "How are we able to breath under water?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Estuaries" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
                        break;
                    case "DeepCoralReef":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "The GroundCrashers are getting alot stronger.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I also think they change if we figth the same lvl again.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "idk it could be the case.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "DeepCoralReef" });
                        break;
                    case "HydroVent":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the second chaos orb.", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Lets go fast.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "First lets fight these GroundCrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        break;
                    case "HydroVentLVL4":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I will encage it again. get back", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrb.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged1.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbcaged.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "this one wont spread chaos anymore.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "He didnt know how", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "₩ЯØП₲", ImagePathPortrait = "", backgroundImage = "hardcore_marine" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "he was", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        break;
                    case "Desert":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "The last one has to be in the vulcano right?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Desert" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I think so two. we have to go quick. what if it is more unsteable because of the heat", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Desert" });
                        break;
                    case "Volcano":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Is that a rocket?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "We have to go to space. What if something is there.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "could be. lets first search the vulcano. Maybe there is one here two", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                        break;
                    case "VolcanoLVL5":
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
                        break;
                    case "Earth":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I always wanted to go to space. now i am finaly here", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Earth" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "How are we able to breath in space??", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Earth" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice", ImagePathPortrait = "Images/portraits/Aria.png", backgroundImage = "Earth" });
                        break;
                    case "Moon":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What is a titan?! they are so strong.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Moon" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "yeah, but we need to fight them.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Moon" });
                        break;
                    case "Sun":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "He has to be here. He has to.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Who do you mean?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "He will be so proud of me.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What do you mean?!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    case "SunLVL2":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see him.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Who do you mean? Please tell me.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I have to go.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Dont leave me here With these titans!", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    case "SunLVL3":
                        dialogLines.Add(new DialogLine { Character = "God", Text = "Good job my servent Aria. you did well", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "God", Text = "But you have no use for me now", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                        ariaKilled = true; // Aria dies in this part of the story
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Urgh…", ImagePathPortrait = "Images/portraits/ariakilled.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Noooooooo.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    default:
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "This biome is still under construction.", ImagePathPortrait = "", backgroundImage = "construction" });
                        break;
                }
            }
            else
            {
                switch (biomeName)
                {
                    case "Forest":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There once was a tale of 3 Chaos Orbs, ancient relics that normally protect these lands.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "But Aria received a vision—a warning that the Orbs would shatter, and chaos would seep into the world.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "She confided in Rook, and together they set out to prevent the looming disaster.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "This forest feels... alive. Like it's watching us.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What happend?! Did the chaos orb break?.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What happend to you? Please dont be dead.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethings comming from behind those trees.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "there it was a GroundCrasher. luckily Aria was prepared.", ImagePathPortrait = "", backgroundImage = "Forest" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Get behind me Rook! i will handel this.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Forest" });
                        break;
                    case "Wasteland":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Aria, please help me. i cant do this alone.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Wasteland" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Not exactly. But we have to keep moving—and I have a feeling there’s something inside that cavern.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Wasteland" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "What they didn’t know was that in the three biomes ahead, more GroundCrashers awaited.", ImagePathPortrait = "", backgroundImage = "Wasteland" });
                        break;
                    case "Mountain":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I cant beleve it. you worked for God?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i only know that GroundCrashers primary-type's have other types they are strong/weak against. not that biomes do debuffs.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "to find out more information abount GroundCrashers go to: https://stan.1pc.nl/GroundCrashers/Website/index.html", ImagePathPortrait = "", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i also know that sommethimes you need multiple GroundCrashers so it might be time to go to the gamble store for more groundcrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Mountain" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the gamble store can be found by pressing the button in the left top of the map.", ImagePathPortrait = "", backgroundImage = "Mountain" });
                        break;
                    case "CrystalCavern":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Were finaly in the cavern. i know there has to be something here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "you lied to me. this was all God's plan right?.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        break;
                    case "CrystalCavernLVL5":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "They have been looking for hours but coudnt find anything..", ImagePathPortrait = "", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What even are you. I didnt think ghost exist.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "i am starting to doubt myself two. lets leave", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "AAAAAAHHH", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "What am I meant to do? I dont know how to save the world on my own.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CrystalCavern" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "they both went down a hidden trapdoor. heading deeper into the cave", ImagePathPortrait = "", backgroundImage = "hardcore_cave" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the cave map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "hardcore_cave" });
                        break;
                    case "Catacomb":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I knew I was right. I know it is here, lets go", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Did the chaos orb break just now?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Catacomb" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "There is no time for that we have to fight.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Catacomb" });
                        break;
                    case "CaveCitadel":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I believed in you.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I normaly never trust anyone.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "The one time i do this happens.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "this is all your fault.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "i dont know what i am meant to do.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveCitadel" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think it has to mean something. but lets fight these GroundCrashers first.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveCitadel" });
                        break;
                    case "CaveLake":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Could the awnser be lake?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "CaveLake" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "How could you lie to me like that.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "CaveLake" });
                        break;
                    case "FungalHollow":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It has to mean something else, we have been searching this place for houres now.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "We arnt even at the lake anymore.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "FungalHollow" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I still dont even know how you are still here", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "FungalHollow" });
                        break;
                    case "Altar":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "There it is the first chaos orb.", ImagePathPortrait = "", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "You where killed.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "No i dont. you defend against the GroundCrashers. i will figure out what to do with it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        break;
                    case "AltarLVL4":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I figured it out. i know how to encage it", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Let me work my magic.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Why do i even bother?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbBroken.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbDelete.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbGone.png", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "and so the first", ImagePathPortrait = "", backgroundImage = "Altar" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "m¡$†@k€.", ImagePathPortrait = "", backgroundImage = "cave" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "was made.", ImagePathPortrait = "", backgroundImage = "Altar" });
                        break;
                    case "Highlands":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I hate you.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I hope you know that.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Highlands" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It does make sence. we can search there when we get to it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Highlands" });
                        break;
                    case "Savanna":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "somethimes these fights are just unfair.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Savanna" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I will never forgive you.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I hope you know that.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Savanna" });
                        break;
                    case "Ocean":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "If we are right about the awnser of the riddle. than it should be here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Just go away.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets look at the surface first. Than we can go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        break;
                    case "OceanLVL5":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Lets go deeper.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Let me guess it is going to break right when I get there.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I dont know what i am doing here.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Ocean" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the marine map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "hardcore_marine" });
                        break;
                    case "Estuaries":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Look how big it is down here.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I dont understand anything anymore.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Estuaries" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Estuaries" });
                        break;
                    case "DeepCoralReef":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I dont even know who i am talking to.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Your just a ghost.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "DeepCoralReef" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "idk it could be the case.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "DeepCoralReef" });
                        break;
                    case "HydroVent":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see it.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the second chaos orb.", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "History is just repeating itself again", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "First lets fight these GroundCrashers.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        break;
                    case "HydroVentLVL4":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I will encage it again. get back", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbBroken.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbDelete.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbGone.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "And again.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "He didnt know how", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "₩ЯØП₲", ImagePathPortrait = "", backgroundImage = "marine" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "he was", ImagePathPortrait = "", backgroundImage = "HydroVent" });
                        break;
                    case "Desert":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "The last one has to be in the vulcano right?.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Desert" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "And again", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Desert" });
                        break;
                    case "Volcano":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "Like it is constandly playing on loop", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "We have to go to space. What if something is there.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "This was all just a big mistake", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                        break;
                    case "VolcanoLVL5":
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "the third orb", ImagePathPortrait = "", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "there is an orb.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I never should have trusted you", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbBroken.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbDelete.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "ChaosOrb", Text = "", ImagePathPortrait = "Images/portraits/ChaosOrbGone.png", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "He was right. this one ¡$ going to", ImagePathPortrait = "", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "฿®£∆k", ImagePathPortrait = "", backgroundImage = "map" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "any minute.", ImagePathPortrait = "", backgroundImage = "Volcano" });
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "You can now acces the space map by the dropdown in the top right corner in the map.", ImagePathPortrait = "", backgroundImage = "hardcore_space" });
                        break;
                    case "Earth":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I always wanted to go to space. now i am finaly here", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Earth" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "We are just going to fight god again. but now i am the one getting killed.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Earth" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "It looks so nice", ImagePathPortrait = "Images/portraits/Aria.png", backgroundImage = "Earth" });
                        break;
                    case "Moon":
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "i almost forgot how much i hated them", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Moon" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "yeah, but we need to fight them.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Moon" });
                        break;
                    case "Sun":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "He has to be here. He has to.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "I could have never seen it comming.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "He will be so proud of me.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "you", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    case "SunLVL2":
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I think i see him.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "A servent of God.", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "I have to go.", ImagePathPortrait = "Images/portraits/aria.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "How could you?", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    case "SunLVL3":
                        dialogLines.Add(new DialogLine { Character = "God", Text = "so you finaly returned, i am happy for you", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "God", Text = "But now it is your time to die Rook", ImagePathPortrait = "Images/GroundCrasherSprites/pattje72.png", backgroundImage = "Sun" });
                        ariaKilled = true; // Aria dies in this part of the story
                        dialogLines.Add(new DialogLine { Character = "Aria", Text = "Urgh…", ImagePathPortrait = "Images/portraits/ariakilled.png", backgroundImage = "Sun" });
                        dialogLines.Add(new DialogLine { Character = "Rook", Text = "No", ImagePathPortrait = "Images/portraits/rook.png", backgroundImage = "Sun" });
                        break;
                    default:
                        dialogLines.Add(new DialogLine { Character = "Narrator", Text = "This biome is still under construction.", ImagePathPortrait = "", backgroundImage = "construction" });
                        break;
                }
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
                if (ariaDead && ariaKilled) CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/ariadeadkilled.png", UriKind.Relative));
                else if (ariaKilled) CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/ariakilled.png", UriKind.Relative));
                else if (ariaDead) CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/ariadead.png", UriKind.Relative));
                else CharacterImage.Source = new BitmapImage(new Uri("Images/portraits/aria.png", UriKind.Relative));

                if (line.ImagePathPortrait == "Images/portraits/ChaosOrb.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrb.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged1.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged1.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbcaged.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbcaged.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbDelete.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/chaosorbdelete.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbBroken.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/ChaosOrbBroken.png", UriKind.Relative)); }
                else if (line.ImagePathPortrait == "Images/portraits/ChaosOrbGone.png") { CharacterImage2.Source = new BitmapImage(new Uri("Images/portraits/chaosorbgone.png", UriKind.Relative)); }
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
                             line.backgroundImage == "hardcore_map" || line.backgroundImage == "space" || line.backgroundImage == "map" || line.backgroundImage == "hardcore_space";
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
