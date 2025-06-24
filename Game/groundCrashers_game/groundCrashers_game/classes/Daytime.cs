using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    public enum Daytimes
    {
        Dawn,
        Day,
        Dusk,
        Night
    }

    public class Daytime
    {
        public Daytimes Name { get; set; }
        public PrimaryEffect Buff { get; set; }
        public PrimaryEffect Debuff { get; set; }
    }

    public class PrimaryEffect
    {
        public List<Primaries> Types { get; set; }
        public string EffectType { get; set; } // e.g., "Attack", "Speed", etc.
    }

    public class DaytimeData
    {
        public string Name { get; set; }
        public DaytimeEffectData Buff { get; set; }
        public DaytimeEffectData Debuff { get; set; }
    }

    public class DaytimeEffectData
    {
        public List<string> Types { get; set; } = new();
        public string Effect { get; set; }
    }

    public static class DaytimeChart
    {
        private static Dictionary<Daytimes, Daytime> _chart = new();
        private static bool _isLoaded = false;
        public static Daytimes currentDaytime { get; set; }

        public static Dictionary<Daytimes, Daytime> Chart
        {
            get
            {
                if (!_isLoaded)
                {
                    //LoadDaytimesFromJson();

                    // uses the internet
                    LoadDaytimesFromWebAsync();
                }
                return _chart;
            }
        }

        public static void LoadDaytimesFromJson()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "daytime.json");

                if (!File.Exists(path))
                    throw new FileNotFoundException($"Daytime JSON not found at path: {path}");

                var text = File.ReadAllText(path);
                var daytimeConfig = JsonConvert.DeserializeObject<List<DaytimeData>>(text);

                if (daytimeConfig == null)
                    throw new Exception("Invalid daytime configuration");

                _chart.Clear();

                foreach (var dt in daytimeConfig)
                {
                    if (Enum.TryParse<Daytimes>(dt.Name, out var dtEnum))
                    {
                        var day = new Daytime
                        {
                            Name = dtEnum,
                            Buff = new PrimaryEffect
                            {
                                Types = dt.Buff.Types.Select(t => Enum.Parse<Primaries>(t)).ToList(),
                                EffectType = dt.Buff.Effect
                            },
                            Debuff = new PrimaryEffect
                            {
                                Types = dt.Debuff.Types.Select(t => Enum.Parse<Primaries>(t)).ToList(),
                                EffectType = dt.Debuff.Effect
                            }
                        };

                        _chart[dtEnum] = day;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load daytime from JSON: {ex.Message}", ex);
            }
        }

        public static async Task LoadDaytimesFromWebAsync()
        {
            try
            {
                string url = "https://stan.1pc.nl/GroundCrashers/data/daytime.json";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download JSON. Status code: {response.StatusCode}");

                    string text = await response.Content.ReadAsStringAsync();

                    var daytimeConfig = JsonConvert.DeserializeObject<List<DaytimeData>>(text);

                    if (daytimeConfig == null)
                        throw new Exception("Invalid daytime configuration");

                    _chart.Clear();

                    foreach (var dt in daytimeConfig)
                    {
                        if (Enum.TryParse<Daytimes>(dt.Name, out var dtEnum))
                        {
                            var day = new Daytime
                            {
                                Name = dtEnum,
                                Buff = new PrimaryEffect
                                {
                                    Types = dt.Buff.Types.Select(t => Enum.Parse<Primaries>(t)).ToList(),
                                    EffectType = dt.Buff.Effect
                                },
                                Debuff = new PrimaryEffect
                                {
                                    Types = dt.Debuff.Types.Select(t => Enum.Parse<Primaries>(t)).ToList(),
                                    EffectType = dt.Debuff.Effect
                                }
                            };

                            _chart[dtEnum] = day;
                        }
                    }

                    _isLoaded = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load daytime from web JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadDaytime()
        {
            _isLoaded = false;
            //LoadDaytimesFromJson();

            LoadDaytimesFromWebAsync();
        }

        public static bool IsLoaded => _isLoaded;

        /// <summary>
        /// get the effectiveness of the current daytime for a creature
        /// </summary>
        /// <param name="creature">the creature</param>
        /// <returns>buff/debuff and the stat that gets affected</returns>
        public static string GetDaytimeEffectiveness(Creature creature)
        {
            if (!_isLoaded)
                //LoadDaytimesFromJson();

                LoadDaytimesFromWebAsync();

            if (_chart.TryGetValue(currentDaytime, out var daytime))
            {
                // Check if the creature's primary type is affected by the current daytime
                if (daytime.Buff.Types.Contains(creature.primary_type)) return $"Buff/{daytime.Buff.EffectType}";
                // If the creature's primary type is not buffed, check if it is debuffed
                else if (daytime.Debuff.Types.Contains(creature.primary_type)) return $"Debuff/{daytime.Debuff.EffectType}";
            }

            // If no effect is found, return "none/none"
            return "none/none";
        }
    }
}
