using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    public class LevelData
    {
        public string Name { get; set; }
        public Biomes Biome { get; set; }
        public Daytimes Daytime { get; set; }
        public int AmountCreaturesPlayer { get; set; }
        public int AmountCreaturesCpu { get; set; }
        public List<int> CreatureID { get; set; }
        public int StrengthModifier { get; set; }
    }

    public static class Levels
    {
        private static Dictionary<string, LevelData> _levels = new();
        private static bool _isLoaded = false;

        public static Dictionary<string, LevelData> Chart
        {
            get
            {
                if (!_isLoaded)
                {
                    // Attempt to load web version
                    LoadLevelsFromWebAsync().Wait();
                }
                return _levels;
            }
        }

        public static async Task LoadLevelsFromWebAsync()
        {
            try
            {
                string url = "https://stan.1pc.nl/GroundCrashers/data/levels.json";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Web request failed with status code: {response.StatusCode}");

                    string json = await response.Content.ReadAsStringAsync();
                    LoadLevelData(json);
                    _isLoaded = true;
                }
            }
            catch (Exception webEx)
            {
                Console.WriteLine($"Web load failed: {webEx.Message}. Trying local file...");
                try
                {
                    LoadLevelsFromJson();
                }
                catch (Exception fileEx)
                {
                    throw new Exception("Failed to load levels from both web and local JSON.", fileEx);
                }
            }
        }

        public static void LoadLevelsFromJson()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "levels.json");

            if (!File.Exists(path))
                throw new FileNotFoundException($"Local level file not found at: {path}");

            string json = File.ReadAllText(path);
            LoadLevelData(json);
            _isLoaded = true;
        }

        private static void LoadLevelData(string json)
        {
            var levelList = JsonConvert.DeserializeObject<List<RawLevelData>>(json);
            if (levelList == null)
                throw new Exception("Invalid level configuration");

            _levels.Clear();

            foreach (var raw in levelList)
            {
                if (!Enum.TryParse<Biomes>(raw.Biome.Replace(" ", ""), out var biomeEnum))
                    continue;

                if (!Enum.TryParse<Daytimes>(raw.Daytime, out var daytimeEnum))
                    continue;

                var level = new LevelData
                {
                    Name = raw.Name,
                    Biome = biomeEnum,
                    Daytime = daytimeEnum,
                    AmountCreaturesPlayer = raw.AmountCreaturesPlayer,
                    AmountCreaturesCpu = raw.AmountCreaturesCpu,
                    CreatureID = raw.CreatureID ?? new List<int>(),
                    StrengthModifier = raw.StrengthModifier
                };

                _levels[level.Name] = level;
            }
        }

        public static void ReloadLevels()
        {
            _isLoaded = false;
            LoadLevelsFromWebAsync();
        }

        public static bool IsLoaded => _isLoaded;

        private class RawLevelData
        {
            public string Name { get; set; }
            public string Biome { get; set; }
            public string Daytime { get; set; }
            public int AmountCreaturesPlayer { get; set; }
            public int AmountCreaturesCpu { get; set; }
            public List<int> CreatureID { get; set; }
            public int StrengthModifier { get; set; }
        }
    }
}