using DocumentFormat.OpenXml.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                    LoadWeathersFromJson();

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

        public static void ReloadWeather()
        {
            _isLoaded = false;
            LoadWeathersFromJson();
        }

        public static bool IsLoaded => _isLoaded;

        public static string GetWeatherEffectiveness(Creature creature)
        {
            if (!_isLoaded)
            {
                LoadWeathersFromJson();
            }

            if (_chart.TryGetValue(currentWeather, out var weather))
            {
                if (weather.Buff.Types.Contains(creature.element)) return $"Buff/{weather.Buff.EffectType}"; // Example: "Buff/Attack"
                else if (weather.Debuff.Types.Contains(creature.element)) return $"Debuff/{weather.Debuff.EffectType}"; // Example: "Debuff/Health
            }

            return "none/none"; // No effect
        }
    }
}