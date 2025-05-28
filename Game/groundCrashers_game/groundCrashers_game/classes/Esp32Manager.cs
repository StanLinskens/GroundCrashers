using System;
using System.IO.Ports;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    public class Esp32Manager
    {
        private readonly HttpClient _client;
        private readonly string _esp32Url;
        private readonly int _baudRate;

        public Esp32Manager(string esp32Url = "http://192.168.230.27", int baudRate = 115200)
        {
            _client = new HttpClient();
            _esp32Url = esp32Url;
            _baudRate = baudRate;
        }

        public async Task<string> GetUidAsync()
        {
            try
            {
                return await _client.GetStringAsync(_esp32Url);
            }
            catch (Exception ex)
            {
                return $"Fout bij verbinden:\n{ex.Message}";
            }
        }

        public async Task<string> SendCredentialsAsync(string ssid, string password)
        {
            if (string.IsNullOrWhiteSpace(ssid) || string.IsNullOrWhiteSpace(password))
                return "SSID of wachtwoord mag niet leeg zijn.";

            try
            {
                // Serial
                string[] portNames = SerialPort.GetPortNames();
                if (portNames.Length == 0)
                    return "Geen seriële poorten gevonden.";

                string myPortName = portNames[0]; // you might make this configurable
                using (SerialPort sp = new SerialPort(myPortName, _baudRate))
                {
                    sp.Open();
                    sp.WriteLine(ssid);
                    sp.WriteLine(password);
                    sp.Close();
                }

                // HTTP
                var content = new StringContent(
                    $"ssid={ssid}&password={password}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                HttpResponseMessage response = await _client.PostAsync(_esp32Url + "/credentials", content);
                string result = await response.Content.ReadAsStringAsync();

                return $"Verzonden!\nAntwoord ESP32:\n{result}";
            }
            catch (Exception ex)
            {
                return $"Fout bij verzenden:\n{ex.Message}";
            }
        }
    }
}
