using Newtonsoft.Json;
using System.IO;

namespace groundCrashers_game
{
    public enum Biomes
    {
        Forest,
        Desert,
        Mountain,
        Highlands,
        Glacier,
        Swamp,
        Ocean,
        Volcano,
        Savanna,
        Jungle,
        Tundra,
        Cave,
        Ruins,
        Marsh,
        CrystalCavern,
        Wasteland
    }


    public class Biome
    {
        public Biomes Name { get; set; }
        public Effect Buff { get; set; }
        public Effect Debuff { get; set; }
    }

    public class BiomeData
    {
        public string Name { get; set; }
        public WeatherEffectData Buff { get; set; }
        public WeatherEffectData Debuff { get; set; }
    }

    public static class BiomeChart
    {
        private static Dictionary<Biomes, Biome> _chart = new();
        private static bool _isLoaded = false;
        public static Biomes currentBiome { get; set; }

        public static Dictionary<Biomes, Biome> Chart
        {
            get
            {
                if (!_isLoaded)
                    LoadBiomesFromJson();
                return _chart;
            }
        }

        public static void LoadBiomesFromJson()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "biomes.json");

                if (!File.Exists(path))
                    throw new FileNotFoundException($"Biome JSON not found at path: {path}");

                var text = File.ReadAllText(path);
                var biomeConfig = JsonConvert.DeserializeObject<List<BiomeData>>(text);

                if (biomeConfig == null)
                    throw new Exception("Invalid biome configuration");

                _chart.Clear();

                foreach (var biomeData in biomeConfig)
                {
                    if (Enum.TryParse<Biomes>(biomeData.Name.Replace(" ", ""), out var biomeEnum))
                    {
                        var biome = new Biome
                        {
                            Name = biomeEnum,
                            Buff = new Effect
                            {
                                Types = biomeData.Buff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                EffectType = biomeData.Buff.Effect
                            },
                            Debuff = new Effect
                            {
                                Types = biomeData.Debuff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                EffectType = biomeData.Debuff.Effect
                            }
                        };

                        _chart[biomeEnum] = biome;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load biome from JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadBiomes()
        {
            _isLoaded = false;
            LoadBiomesFromJson();
        }

        public static bool IsLoaded => _isLoaded;

        public static string GetBiomeEffectiveness(Creature creature)
        {
            if (!_isLoaded)
                LoadBiomesFromJson();

            if (_chart.TryGetValue(currentBiome, out var biome))
            {
                if (biome.Buff.Types.Contains(creature.element)) return $"Buff/{biome.Buff.EffectType}";
                else if (biome.Debuff.Types.Contains(creature.element)) return $"Debuff/{biome.Debuff.EffectType}";
            }

            return "none/none";
        }
    }
}
