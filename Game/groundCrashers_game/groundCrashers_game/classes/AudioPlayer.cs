using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace groundCrashers_game.classes
{
public class AudioPlayer
{
    private WindowsMediaPlayer player = new WindowsMediaPlayer();
    private Random random = new Random();

    public void PlayRandomBattleMusic()
    {
        string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../", "../", "../", "Audio");

        if (!Directory.Exists(folder))
        {
            Console.WriteLine("Audio folder not found: " + folder);
            return;
        }

        var files = Directory.GetFiles(folder, "battleground*.mp3");

        if (files.Length == 0)
        {
            Console.WriteLine("No MP3 files found in: " + folder);
            return;
        }

            string musicFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../", "../", "../", "Audio");
            string[] mp3Files = Directory.GetFiles(musicFolder, "battleground*.mp3");

            if (mp3Files.Length > 0)
            {
                string randomFile = mp3Files[new Random().Next(mp3Files.Length)];
                WindowsMediaPlayer player = new WindowsMediaPlayer();
                player.URL = randomFile;
                player.controls.play();
            }

        }
    }
}