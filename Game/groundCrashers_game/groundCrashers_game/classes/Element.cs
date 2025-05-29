using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    public enum Elements
    {
        Nature,
        Ice,
        Toxic,
        Fire,
        Water,
        Draconic,
        Earth,
        Dark,
        Wind,
        Psychic,
        Light,
        GOD,
        Demonic,
        Electric,
        Acid,
        Magnetic,
        ALL,
        none,
        Chaos,
        Cosmic,
        Void,
        Astral
    }

    public class Element
    {
        public Elements Name { get; set; }
        public List<Elements> StrongAgainst { get; set; } = new();
        public List<Elements> WeakAgainst { get; set; } = new();
    }

    // JSON structure classes for deserialization
    public class ElementData
    {
        public string Name { get; set; }
        public List<string> StrongAgainst { get; set; } = new();
        public List<string> WeakAgainst { get; set; } = new();
    }

    public class ElementsConfig
    {
        public List<ElementData> Elements { get; set; } = new();
    }

    public static class ElementChart
    {
        private static Dictionary<Elements, Element> _chart = new();
        private static bool _isLoaded = false;

        public static Dictionary<Elements, Element> Chart
        {
            get
            {
                if (!_isLoaded)
                {
                    //LoadElementsFromJson();

                    // uses the internet
                    LoadElementsFromWebAsync();
                }
                return _chart;
            }
        }

        public static void LoadElementsFromJson()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "elements.json");

                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Elements JSON not found at path: {path}");
                }

                var text = File.ReadAllText(path);
                var elementsConfig = JsonConvert.DeserializeObject<ElementsConfig>(text);

                if (elementsConfig == null || elementsConfig.Elements == null)
                {
                    throw new Exception("Invalid elements configuration");
                }

                _chart.Clear();

                foreach (var elementData in elementsConfig.Elements)
                {
                    // Parse the element name to enum
                    if (Enum.TryParse<Elements>(elementData.Name, out Elements elementEnum))
                    {
                        var element = new Element
                        {
                            Name = elementEnum,
                            StrongAgainst = new List<Elements>(),
                            WeakAgainst = new List<Elements>()
                        };

                        // Parse StrongAgainst elements
                        foreach (var strongName in elementData.StrongAgainst)
                        {
                            if (Enum.TryParse<Elements>(strongName, out Elements strongEnum))
                            {
                                element.StrongAgainst.Add(strongEnum);
                            }
                        }

                        // Parse WeakAgainst elements
                        foreach (var weakName in elementData.WeakAgainst)
                        {
                            if (Enum.TryParse<Elements>(weakName, out Elements weakEnum))
                            {
                                element.WeakAgainst.Add(weakEnum);
                            }
                        }

                        _chart[elementEnum] = element;
                    }
                }

                _isLoaded = true;
            }
            catch (Exception ex)
            {
                // If loading fails, fall back to hardcoded values or throw
                throw new Exception($"Failed to load elements from JSON: {ex.Message}", ex);
            }
        }

        public static async Task LoadElementsFromWebAsync()
        {
            try
            {
                string url = "https://stan.1pc.nl/GroundCrashers/data/elements.json";

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download JSON. Status code: {response.StatusCode}");

                    string text = await response.Content.ReadAsStringAsync();

                    var elementsConfig = JsonConvert.DeserializeObject<ElementsConfig>(text);

                    if (elementsConfig == null || elementsConfig.Elements == null)
                    {
                        throw new Exception("Invalid elements configuration");
                    }

                    _chart.Clear();

                    foreach (var elementData in elementsConfig.Elements)
                    {
                        if (Enum.TryParse<Elements>(elementData.Name, out Elements elementEnum))
                        {
                            var element = new Element
                            {
                                Name = elementEnum,
                                StrongAgainst = new List<Elements>(),
                                WeakAgainst = new List<Elements>()
                            };

                            foreach (var strongName in elementData.StrongAgainst)
                            {
                                if (Enum.TryParse<Elements>(strongName, out Elements strongEnum))
                                {
                                    element.StrongAgainst.Add(strongEnum);
                                }
                            }

                            foreach (var weakName in elementData.WeakAgainst)
                            {
                                if (Enum.TryParse<Elements>(weakName, out Elements weakEnum))
                                {
                                    element.WeakAgainst.Add(weakEnum);
                                }
                            }

                            _chart[elementEnum] = element;
                        }
                    }

                    _isLoaded = true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load elements from web JSON: {ex.Message}", ex);
            }
        }

        public static void ReloadElements()
        {
            _isLoaded = false;
            //LoadElementsFromJson();

            LoadElementsFromWebAsync();

        }

        public static float GetElementEffectiveness(Elements attacker, Elements defender)
        {
            if (!_isLoaded)
            {
                //LoadElementsFromJson();

                LoadElementsFromWebAsync();
            }

            if (_chart.ContainsKey(attacker))
            {
                if (_chart[attacker].StrongAgainst.Contains(defender)) return 1.5f;
                else if (_chart[attacker].WeakAgainst.Contains(defender)) return 0.5f;
            }

            return 1.0f;
        }

        // Helper method to get all loaded elements (useful for debugging)
        public static List<Elements> GetAllElements()
        {
            if (!_isLoaded)
            {
                //LoadElementsFromJson();

                LoadElementsFromWebAsync();
            }
            return _chart.Keys.ToList();
        }

        // Helper method to check if elements are loaded
        public static bool IsLoaded => _isLoaded;
    }
}