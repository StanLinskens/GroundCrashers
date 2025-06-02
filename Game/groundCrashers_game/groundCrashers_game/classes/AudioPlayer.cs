using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    public class AudioPlayer : IDisposable
    {
        private static AudioPlayer _instance;
        public static AudioPlayer Instance => _instance ??= new AudioPlayer();

        private SoundPlayer player;            // for PlaySpecific
        private SoundPlayer battlePlayer;      // for the battleground loop
        private readonly Random random = new Random();

        // Token source used to stop the battleground rotation thread
        private CancellationTokenSource battleCts;
        private Task battleTask;
        private readonly object lockObject = new object();

        /// <summary>
        /// Play any specific .wav file once or looped.
        /// </summary>
        /// <param name="fileName">File name (just the .wav file, no path)</param>
        /// <param name="loop">If true, will loop until Stop() is called</param>
        public void PlaySpecific(string fileName, bool loop = false)
        {
            // Build full path from fileName (just the file, no path)
            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Audio");
            string fullPath = Path.GetFullPath(Path.Combine(folder, fileName));

            if (!File.Exists(fullPath))
            {
                Console.WriteLine($"File not found: {fullPath}");
                return;
            }

            Stop(); // stop any existing sound or battleground thread

            try
            {
                lock (lockObject)
                {
                    player = new SoundPlayer(fullPath);

                    if (loop)
                    {
                        player.PlayLooping();
                        Console.WriteLine($"Looping: {fullPath}");
                    }
                    else
                    {
                        player.Play();
                        Console.WriteLine($"Playing once: {fullPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound '{fullPath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Start continuously rotating through all battleground*.wav files in the Audio folder.
        /// When one finishes, another (random) will start automatically.
        /// This runs asynchronously and won't block the UI.
        /// </summary>
        public async Task PlayRandomBattleMusicAsync()
        {
            // Stop any sound that might already be playing
            await StopAsync();

            // Locate your Audio folder
            string folder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "Audio"
            );
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Audio folder not found: " + folder);
                return;
            }

            // Gather all files that match battleground*.wav
            string[] wavFiles = Directory.GetFiles(folder, "battleground*.wav");
            if (wavFiles.Length == 0)
            {
                Console.WriteLine("No battleground WAV files found in: " + folder);
                return;
            }

            // Pick one file at random
            string chosenFile = wavFiles[random.Next(wavFiles.Length)];
            Console.WriteLine("Playing once: " + chosenFile);

            try
            {
                lock (lockObject)
                {
                    // Dispose any previous players
                    player?.Dispose();

                    // Set up a new SoundPlayer for the chosen file
                    player = new SoundPlayer(chosenFile);
                    player.Play(); // Play once, no looping
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound '{chosenFile}': {ex.Message}");
            }
        }

        // Synchronous wrapper (you can keep this as-is or remove it if you don’t need a sync call)
        public void PlayRandomBattleMusic()
        {
            _ = PlayRandomBattleMusicAsync();
        }


        /// <summary>
        /// Stops any playing sound (specific or battleground loop) and cancels the loop.
        /// </summary>
        public void Stop()
        {
            StopAsync().Wait();
        }

        /// <summary>
        /// Async version of Stop method
        /// </summary>
        public async Task StopAsync()
        {
            // 1) Cancel the battleground loop if it's running
            if (battleCts != null && !battleCts.IsCancellationRequested)
            {
                battleCts.Cancel();

                // Wait for the task to complete with timeout
                if (battleTask != null)
                {
                    try
                    {
                        await battleTask.WaitAsync(TimeSpan.FromSeconds(2));
                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Battle task didn't stop within timeout");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error stopping battle task: {ex.Message}");
                    }
                }

                battleCts.Dispose();
                battleCts = null;
                battleTask = null;
            }

            lock (lockObject)
            {
                // 2) Stop/Dispose the battlePlayer if it exists
                if (battlePlayer != null)
                {
                    try
                    {
                        battlePlayer.Stop();
                    }
                    catch { /* ignore errors */ }

                    battlePlayer.Dispose();
                    battlePlayer = null;
                }

                // 3) Stop/Dispose the "specific" player if it exists
                if (player != null)
                {
                    try
                    {
                        player.Stop();
                    }
                    catch { /* ignore errors */ }

                    player.Dispose();
                    player = null;
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }

        private int GetAudioDurationMs(string filePath)
        {
            // Use NAudio to read the file's duration
            try
            {
                using (var reader = new NAudio.Wave.AudioFileReader(filePath))
                {
                    return (int)reader.TotalTime.TotalMilliseconds;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting audio duration for '{filePath}': {ex.Message}");
                return 10000; // Default to 10 seconds if unknown
            }
        }

    }
}