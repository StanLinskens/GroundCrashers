﻿using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    public enum Biomes
    {
        // normal map
        Forest,
        Wasteland,
        Mountain,
        Glacier,
        Jungle,
        CrystalCavern,

        // underground
        Catacomb,
        LavaChamber,
        CaveCitadel,
        CaveLake,
        FungalHollow,
        Dungeon,
        CaveVillage,
        Altar,

        // back to normal map
        Swamp,
        Highlands,
        Marsh,
        Tundra,
        Savanna,
        Ruins,
        Ocean,

        // marine
        Estuaries,
        CoralReef,
        OpenOcean,
        DeepCoralReef,
        ColdSeep,
        HydroVent,

        // back to normal map
        Desert,
        Volcano,

        // space map
        Earth,
        Moon,
        Nebula,
        Interstellar,
        Debris,
        Saturn,
        Cybertron,
        Asteroids,
        Sun,
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
                {
                    //LoadBiomesFromJson();

                    // uses the internet
                    LoadBiomesFromWebAsync();
                }
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

        public static async Task LoadBiomesFromWebAsync()
        {
            try
            {
                string url = "https://stan.1pc.nl/GroundCrashers/data/biomes.json";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download JSON. Status code: {response.StatusCode}");

                    string text = await response.Content.ReadAsStringAsync();

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
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load biomes from web JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadBiomes()
        {
            _isLoaded = false;
            //LoadBiomesFromJson();

            LoadBiomesFromWebAsync();
        }

        public static bool IsLoaded => _isLoaded;

        /// <summary>
        /// gets the effectiveness of a creature in the current biome.
        /// </summary>
        /// <param name="creature">the creature</param>
        /// <returns>buff/debuff and stat that gets effected</returns>
        public static string GetBiomeEffectiveness(Creature creature)
        {
            if (!_isLoaded)
                //LoadBiomesFromJson();

                LoadBiomesFromWebAsync();

            if (_chart.TryGetValue(currentBiome, out var biome))
            {
                // check if the creature's element is in the buff or debuff types of the biome
                if (biome.Buff.Types.Contains(creature.element)) return $"Buff/{biome.Buff.EffectType}";
                // if the creature's element is not buffed, check if it is debuffed
                else if (biome.Debuff.Types.Contains(creature.element)) return $"Debuff/{biome.Debuff.EffectType}";
            }

            // if the biome is not loaded or the creature's element is not affected, return "none/none"
            return "none/none";
        }
    }
}
