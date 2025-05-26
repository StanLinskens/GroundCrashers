using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace groundCrashers_game.classes
{
    // Status Effect Classes
    public class StatusEffect
    {
        [JsonProperty("element")]
        public string Element { get; set; }

        [JsonProperty("statusName")]
        public string StatusName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("effect")]
        public StatusEffectModifier Effect { get; set; }
    }

    public class StatusEffectModifier
    {
        [JsonProperty("stat")]
        public string Stat { get; set; }

        [JsonProperty("modifier")]
        public int Modifier { get; set; }
    }

    public class StatusEffectsData
    {
        [JsonProperty("statusEffects")]
        public List<StatusEffect> StatusEffects { get; set; } = new List<StatusEffect>();
    }

    // Status Effects Manager
    public static class StatusEffectsManager
    {
        private static Dictionary<Elements, StatusEffect> _statusEffectsByElement = new Dictionary<Elements, StatusEffect>();

        public static void LoadStatusEffectsFromJson()
        {
            try
            {
                string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "statusEffects.json");
                if (!File.Exists(path))
                    throw new FileNotFoundException("Status Effects JSON not found.");

                var text = File.ReadAllText(path);
                var statusData = JsonConvert.DeserializeObject<StatusEffectsData>(text);

                _statusEffectsByElement.Clear();

                foreach (var effect in statusData.StatusEffects)
                {
                    if (Enum.TryParse<Elements>(effect.Element, true, out Elements element))
                    {
                        _statusEffectsByElement[element] = effect;
                    }
                    else if (effect.Element.ToLower() == "all")
                    {
                        _statusEffectsByElement[Elements.ALL] = effect;
                    }
                    else if (effect.Element.ToLower() == "none")
                    {
                        _statusEffectsByElement[Elements.none] = effect;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading status effects: {ex.Message}");
            }
        }

        public static StatusEffect GetStatusEffect(Elements element)
        {
            return _statusEffectsByElement.TryGetValue(element, out StatusEffect effect) ? effect : null;
        }

        public static string GetStatusName(Elements element)
        {
            var effect = GetStatusEffect(element);
            return effect?.StatusName ?? "None";
        }

        public static string GetStatusDescription(Elements element)
        {
            var effect = GetStatusEffect(element);
            return effect?.Description ?? "No status effect";
        }

    }
}