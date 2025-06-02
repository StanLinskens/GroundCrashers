using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

namespace groundCrashers_game
{
    public class Esp32Manager
    {
        private readonly HttpClient _client;
        private readonly string _esp32BaseUrl;
        private readonly int _baudRate;

        public Esp32Manager(string esp32BaseUrl = "http://192.168.117.27", int baudRate = 115200)
        {
            _client = new HttpClient();
            _esp32BaseUrl = esp32BaseUrl.TrimEnd('/');
            _baudRate = baudRate;
        }

        /// <summary>
        /// Calls the ESP32 /read endpoint. Waits for a card tap.
        /// Returns either the creature data (single ID or comma-separated IDs) or an error string.
        /// </summary>
        public async Task<string> GetCardDataAsync()
        {
            try
            {
                string fullUrl = $"{_esp32BaseUrl}/read";
                string result = await _client.GetStringAsync(fullUrl);
                return result.Trim();
            }
            catch (Exception ex)
            {
                return $"Error connecting to ESP32: {ex.Message}";
            }
        }

        /// <summary>
        /// Calls the ESP32 /read endpoint and returns a list of creature IDs.
        /// If the card contains "1,2,3,4,5", this returns [1, 2, 3, 4, 5].
        /// </summary>
        public async Task<List<int>> GetCreatureIDsAsync()
        {
            try
            {
                string cardData = await GetCardDataAsync();
                
                // Check for errors`
                if (cardData.StartsWith("Error") || cardData == "NO_CREATURE")
                {
                    Console.WriteLine($"Card read error: {cardData}");
                    return new List<int>();
                }

                // Parse creature IDs
                List<int> creatureIDs = new List<int>();
                string[] idStrings = cardData.Split(',');
                
                foreach (string idStr in idStrings)
                {
                    if (int.TryParse(idStr.Trim(), out int id) && id > 0)
                    {
                        creatureIDs.Add(id);
                    }
                }

                Debug.WriteLine($"Found {creatureIDs.Count} creatures on card: {string.Join(", ", creatureIDs)}");
                return creatureIDs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing creature IDs: {ex.Message}");
                return new List<int>();
            }
        }

        /// <summary>
        /// Gets a single creature ID (the first one if multiple exist on the card).
        /// Maintains backwards compatibility with your existing code.
        /// </summary>
        public async Task<string> GetCardIDAsync()
        {
            var creatureIDs = await GetCreatureIDsAsync();
            if (creatureIDs.Count > 0)
            {
                return creatureIDs[0].ToString();
            }
            return "NO_CREATURE";
        }

        /// <summary>
        /// Writes a single creature ID to a card.
        /// </summary>
        public async Task<string> WriteCardIDAsync(int id)
        {
            if (id <= 0)
                return "ID must be greater than zero.";

            try
            {
                string fullUrl = $"{_esp32BaseUrl}/write?id={id}";
                string response = await _client.GetStringAsync(fullUrl);
                return response.Trim();
            }
            catch (Exception ex)
            {
                return $"Error writing to ESP32: {ex.Message}";
            }
        }

        /// <summary>
        /// Writes multiple creature IDs to a card as comma-separated values.
        /// Example: WriteMultipleCreatureIDsAsync([1, 2, 3, 4, 5]) writes "1,2,3,4,5"
        /// </summary>
        public async Task<string> WriteMultipleCreatureIDsAsync(List<int> creatureIDs)
        {
            if (creatureIDs == null || creatureIDs.Count == 0)
                return "No creature IDs provided.";

            if (creatureIDs.Any(id => id <= 0))
                return "All creature IDs must be greater than zero.";

            try
            {
                // Join IDs with commas
                string combinedIDs = string.Join(",", creatureIDs);
                
                // Note: You'll need to modify the ESP32 /write endpoint to accept comma-separated values
                string fullUrl = $"{_esp32BaseUrl}/write?id={combinedIDs}";
                string response = await _client.GetStringAsync(fullUrl);
                return response.Trim();
            }
            catch (Exception ex)
            {
                return $"Error writing multiple IDs to ESP32: {ex.Message}";
            }
        }

        /// <summary>
        /// Example usage method showing how to work with multiple creatures
        /// </summary>
        public async Task<string> HandleMultipleCreaturesExample()
        {
            var creatures = await GetCreatureIDsAsync();
            
            if (creatures.Count == 0)
            {
                return "No creatures found on card";
            }
            else if (creatures.Count == 1)
            {
                return $"Single creature battle with Creature #{creatures[0]}";
            }
            else
            {
                return $"Multi-creature battle with {creatures.Count} creatures: {string.Join(", ", creatures.Select(id => $"#{id}"))}";
            }
        }
    }
}