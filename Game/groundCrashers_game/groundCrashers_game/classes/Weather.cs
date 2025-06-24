using DocumentFormat.OpenXml.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static groundCrashers_game.classes.Manager;

namespace groundCrashers_game
{
    public enum Weathers
    {
        Sunny,
        Rainy,
        Cloudy,
        Foggy,
        Windy,
        Hail,
        Sandstorm,
        Clear
    }

    public class Weather
    {
        public Weathers Name { get; set; }
        public Effect Buff { get; set; }
        public Effect Debuff { get; set; }
    }

    public class Effect
    {
        public List<Elements> Types { get; set; }
        public string EffectType { get; set; }  // Renamed to avoid conflict
    }

    // JSON structure
    public class WeatherData
    {
        public string Name { get; set; }
        public WeatherEffectData Buff { get; set; }
        public WeatherEffectData Debuff { get; set; }
    }

    public class WeatherEffectData
    {
        public List<string> Types { get; set; } = new();
        public string Effect { get; set; }
    }

    public class WeatherConfig
    {
        public List<WeatherData> Weathers { get; set; } = new();
    }

    public static class WeatherChart
    {
        private static Dictionary<Weathers, Weather> _chart = new();
        private static bool _isLoaded = false;
        public static Weathers currentWeather { get; set; } // Renamed to avoid conflict with Weather class

        public static Dictionary<Weathers, Weather> Chart
        {
            get
            {
                if (!_isLoaded)
                {
                    //LoadWeathersFromJson();

                    // uses the internet
                    LoadWeathersFromWebAsync();
                }

                return _chart;
            }
        }

        public static void LoadWeathersFromJson()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "weather.json");

                if (!File.Exists(path))
                    throw new FileNotFoundException($"Weather JSON not found at path: {path}");

                var text = File.ReadAllText(path);
                var weatherConfig = JsonConvert.DeserializeObject<List<WeatherData>>(text); // assuming your JSON is a list

                if (weatherConfig == null)
                    throw new Exception("Invalid weather configuration");

                _chart.Clear();

                foreach (var weatherData in weatherConfig)
                {
                    if (Enum.TryParse<Weathers>(weatherData.Name, out var weatherEnum))
                    {
                        var weather = new Weather
                        {
                            Name = weatherEnum,
                            Buff = new Effect
                            {
                                Types = weatherData.Buff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                EffectType = weatherData.Buff.Effect
                            },
                            Debuff = new Effect
                            {
                                Types = weatherData.Debuff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                EffectType = weatherData.Debuff.Effect
                            }
                        };

                        _chart[weatherEnum] = weather;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load weather from JSON: {ex.Message}", ex);
            }
        }

        public static async Task LoadWeathersFromWebAsync()
        {
            try
            {
                string url = "https://stan.1pc.nl/GroundCrashers/data/weather.json";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download JSON. Status code: {response.StatusCode}");

                    string text = await response.Content.ReadAsStringAsync();

                    var weatherConfig = JsonConvert.DeserializeObject<List<WeatherData>>(text); // assuming your JSON is a list

                    if (weatherConfig == null)
                        throw new Exception("Invalid weather configuration");

                    _chart.Clear();

                    foreach (var weatherData in weatherConfig)
                    {
                        if (Enum.TryParse<Weathers>(weatherData.Name, out var weatherEnum))
                        {
                            var weather = new Weather
                            {
                                Name = weatherEnum,
                                Buff = new Effect
                                {
                                    Types = weatherData.Buff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                    EffectType = weatherData.Buff.Effect
                                },
                                Debuff = new Effect
                                {
                                    Types = weatherData.Debuff.Types.Select(t => Enum.Parse<Elements>(t)).ToList(),
                                    EffectType = weatherData.Debuff.Effect
                                }
                            };

                            _chart[weatherEnum] = weather;
                        }
                    }

                    _isLoaded = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load weather from web JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadWeather()
        {
            _isLoaded = false;
            //LoadWeathersFromJson();

            LoadWeathersFromWebAsync();
        }

        public static bool IsLoaded => _isLoaded;

        /// <summary>
        /// to get the effectiveness of the current weather on a creature
        /// </summary>
        /// <param name="creature">the creature that gets the buff/debuff </param>
        /// <returns>the buff/debuff or nothing</returns>
        public static string GetWeatherEffectiveness(Creature creature)
        {
            // if the weather is not loaded, load it from web
            if (!_isLoaded)
            {
                //LoadWeathersFromJson();

                LoadWeathersFromWebAsync();
            }
            // get the current weather from the chart
            if (_chart.TryGetValue(currentWeather, out var weather))
            {
                // if the creature's element matches the weather's buff or debuff types, return the effect type
                if (weather.Buff.Types.Contains(creature.element)) return $"Buff/{weather.Buff.EffectType}"; // Example: "Buff/Attack"
                else if (weather.Debuff.Types.Contains(creature.element)) return $"Debuff/{weather.Debuff.EffectType}"; // Example: "Debuff/Health
            }

            return "none/none"; // No effect
        }
    }
}