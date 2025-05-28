using System;
using System.IO.Ports;
using System.Net.Http;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    public class Esp32Manager
    {
        private readonly HttpClient _client;
        private readonly string _esp32BaseUrl;
        private readonly int _baudRate;

        public Esp32Manager(string esp32BaseUrl = "http://192.168.230.27", int baudRate = 115200)
        {
            _client = new HttpClient();
            _esp32BaseUrl = esp32BaseUrl.TrimEnd('/');
            _baudRate = baudRate;
        }

        /// <summary>
        /// Calls the ESP32 /read endpoint. Waits (up to ~10 seconds on the ESP32) for a card tap.
        /// Returns either the creature ID (as string) or an error string.
        /// </summary>
        public async Task<string> GetCardIDAsync()
        {
            try
            {
                string fullUrl = $"{_esp32BaseUrl}/read";
                string result = await _client.GetStringAsync(fullUrl);
                // The ESP32 returns "ERROR: ..." or a number as plain text
                return result.Trim();
            }
            catch (Exception ex)
            {
                return $"Error connecting to ESP32: {ex.Message}";
            }
        }

        /// <summary>
        /// Calls the ESP32 /write endpoint with ?id=X. 
        /// The ESP32 will respond immediately saying it's ready to write,
        /// and then as soon as the next card is tapped, it will write that ID.
        /// </summary>
        public async Task<string> WriteCardIDAsync(int id)
        {
            if (id <= 0)
                return "ID must be greater than zero.";

            try
            {
                // First, send via HTTP: GET /write?id=NNN
                string fullUrl = $"{_esp32BaseUrl}/write?id={id}";
                string response = await _client.GetStringAsync(fullUrl);

                // The ESP32 answer will be something like "OK: Ready to write ID 42"
                return response.Trim();
            }
            catch (Exception ex)
            {
                return $"Error writing to ESP32: {ex.Message}";
            }
        }
    }
}
