using System;
using System.IO;
using System.Media;
using System.Threading;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    public class AudioPlayer
    {
        private SoundPlayer player;
        private Random random = new Random();

        // Token source used to stop the battleground rotation thread
        private CancellationTokenSource battleCts;
        private Task battleTask;

        /// <summary>
        /// Play any specific .wav file once or looped.
        /// </summary>
        /// <param name="filePath">Full path to the .wav file</param>
        /// <param name="loop">If true, will loop the file until Stop() is called</param>
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound '{fullPath}': {ex.Message}");
            }
        }


        /// <summary>
        /// Start continuously rotating through all battleground*.wav files in the Audio folder.
        /// When one finishes, another (random) will start automatically.
        /// </summary>
        public void PlayRandomBattleMusic()
        {
            // Stop anything that might already be playing
            Stop();

            string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Audio");
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("Audio folder not found: " + folder);
                return;
            }

            // Gather all battleground*.wav files
            string[] wavFiles = Directory.GetFiles(folder, "battleground*.wav");
            if (wavFiles.Length == 0)
            {
                Console.WriteLine("No battleground WAV files found in: " + folder);
                return;
            }

            // Set up a CancellationToken so we can stop the loop later
            battleCts = new CancellationTokenSource();
            CancellationToken token = battleCts.Token;

            // Run the rotation in a background Task
            battleTask = Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    // Pick a random battleground file
                    string nextFile = wavFiles[random.Next(wavFiles.Length)];
                    Console.WriteLine("Playing: " + nextFile);

                    try
                    {
                        // Create a new player each time (we’ll dispose it before picking the next)
                        using (var loopPlayer = new SoundPlayer(nextFile))
                        {
                            // PlaySync blocks until the file finishes
                            loopPlayer.PlaySync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error playing '{nextFile}': {ex.Message}");
                    }

                    // Small check to see if we should exit before picking another
                    if (token.IsCancellationRequested) break;
                }
            }, token);
        }

        /// <summary>
        /// Stops any playing sound (specific or in rotation) and cancels the battleground loop.
        /// </summary>
        public void Stop()
        {
            // Cancel the battleground rotation if it’s running
            if (battleCts != null && !battleCts.IsCancellationRequested)
            {
                battleCts.Cancel();
                try
                {
                    battleTask?.Wait(); // wait for the loop to end
                }
                catch (AggregateException) { /* ignore */ }
                battleCts.Dispose();
                battleCts = null;
            }

            // Stop the current player if it exists
            if (player != null)
            {
                try
                {
                    player.Stop();
                }
                catch { /* ignore errors on stop */ }
                player.Dispose();
                player = null;
            }
        }
    }
}
