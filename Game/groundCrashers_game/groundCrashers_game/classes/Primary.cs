using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace groundCrashers_game
{
    public enum Primaries
    {
        Verdant,
        Primal,
        Apex,
        Sapient,
        Synthetic,
        God,
        Titan
    }

    public class Primary
    {
        public Primaries Name { get; set; }
        public List<Primaries> StrongAgainst { get; set; } = new();
        public List<Primaries> BetterAgainst { get; set; } = new();
        public List<Primaries> WeakerAgainst { get; set; } = new();
        public List<Primaries> WeakAgainst { get; set; } = new();
    }

    // JSON structure classes for deserialization
    public class PrimaryData
    {
        public string Name { get; set; }
        public List<string> StrongAgainst { get; set; } = new();
        public List<string> BetterAgainst { get; set; } = new();
        public List<string> WeakerAgainst { get; set; } = new();
        public List<string> WeakAgainst { get; set; } = new();
    }

    public class PrimariesConfig
    {
        public List<PrimaryData> Primaries { get; set; } = new();
    }

    public static class PrimaryChart
    {
        private static Dictionary<Primaries, Primary> _chart = new();
        private static bool _isLoaded = false;

        public static Dictionary<Primaries, Primary> Chart
        {
            get
            {
                if (!_isLoaded)
                {
                    LoadPrimariesFromJson();
                }
                return _chart;
            }
        }

        public static void LoadPrimariesFromJson()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "primaries.json");

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Primaries JSON not found at path: {path}");
                }

                var text = File.ReadAllText(path);
                var primariesConfig = JsonConvert.DeserializeObject<PrimariesConfig>(text);

                if (primariesConfig == null || primariesConfig.Primaries == null)
                {
                    throw new Exception("Invalid primaries configuration");
                }

                _chart.Clear();

                foreach (var primaryData in primariesConfig.Primaries)
                {
                    // Parse the primary name to enum
                    if (Enum.TryParse<Primaries>(primaryData.Name, out Primaries primaryEnum))
                    {
                        var primary = new Primary
                        {
                            Name = primaryEnum,
                            StrongAgainst = new List<Primaries>(),
                            BetterAgainst = new List<Primaries>(),
                            WeakerAgainst = new List<Primaries>(),
                            WeakAgainst = new List<Primaries>()
                        };

                        // Parse StrongAgainst primaries
                        foreach (var strongName in primaryData.StrongAgainst)
                        {
                            if (Enum.TryParse<Primaries>(strongName, out Primaries strongEnum))
                            {
                                primary.StrongAgainst.Add(strongEnum);
                            }
                        }

                        // Parse BetterAgainst primaries
                        foreach (var betterName in primaryData.BetterAgainst)
                        {
                            if (Enum.TryParse<Primaries>(betterName, out Primaries betterEnum))
                            {
                                primary.BetterAgainst.Add(betterEnum);
                            }
                        }

                        // Parse WeakerAgainst primaries
                        foreach (var weakerName in primaryData.WeakerAgainst)
                        {
                            if (Enum.TryParse<Primaries>(weakerName, out Primaries weakerEnum))
                            {
                                primary.WeakerAgainst.Add(weakerEnum);
                            }
                        }

                        // Parse WeakAgainst primaries
                        foreach (var weakName in primaryData.WeakAgainst)
                        {
                            if (Enum.TryParse<Primaries>(weakName, out Primaries weakEnum))
                            {
                                primary.WeakAgainst.Add(weakEnum);
                            }
                        }

                        _chart[primaryEnum] = primary;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                // If loading fails, fall back to hardcoded values or throw
                throw new Exception($"Failed to load primaries from JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadPrimaries()
        {
            _isLoaded = false;
            LoadPrimariesFromJson();
        }

        public static float GetPrimaryEffectiveness(Primaries attacker, Primaries defender)
        {
            if (!_isLoaded)
            {
                LoadPrimariesFromJson();
            }

            if (_chart.ContainsKey(attacker))
            {
                if (_chart[attacker].StrongAgainst.Contains(defender)) return 1.5f;
                else if (_chart[attacker].BetterAgainst.Contains(defender)) return 1.25f;
                else if (_chart[attacker].WeakerAgainst.Contains(defender)) return 0.75f;
                else if (_chart[attacker].WeakAgainst.Contains(defender)) return 0.5f;
            }

            return 1.0f;
        }

        // Helper method to get all loaded primaries (useful for debugging)
        public static List<Primaries> GetAllPrimaries()
        {
            if (!_isLoaded)
            {
                LoadPrimariesFromJson();
            }
            return _chart.Keys.ToList();
        }

        // Helper method to check if primaries are loaded
        public static bool IsLoaded => _isLoaded;
    }
}