using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO.Ports;

namespace groundCrashers_game
{
    public partial class espConnectionWindow : Window
    {
        private readonly HttpClient client = new HttpClient();
        private const string esp32Url = "http://192.168.230.27";

        public espConnectionWindow()
        {
            InitializeComponent();

            // Voorbeeldwaardes, vervang dit met je echte logica
            txtSSID.Text = "GroundCrashersNetwerk";
            txtPASSWORD.Text = "veiligwachtwoord123";
        }

        private async void BtnGetUid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string response = await client.GetStringAsync(esp32Url);
                TxtResult.Text = "RFID Tag UID:\n" + response;
            }
            catch (Exception ex)
            {
                TxtResult.Text = "Fout bij verbinden:\n" + ex.Message;
            }
        }

        public async void BtnSendcredentials_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string ssid = txtSSID.Text.Trim();
                string password = txtPASSWORD.Text.Trim();

                if (string.IsNullOrWhiteSpace(ssid) || string.IsNullOrWhiteSpace(password))
                {
                    TxtResult.Text = "SSID of wachtwoord mag niet leeg zijn.";
                    return;
                }

                // Send over serial
                string[] portNames = SerialPort.GetPortNames();
                if (portNames.Length == 0)
                {
                    TxtResult.Text = "Geen seriële poorten gevonden.";
                    return;
                }

                string myPortName = portNames[0]; // You could let the user choose this
                int baudRate = 115200;

                using (SerialPort sp = new SerialPort(myPortName, baudRate))
                {
                    sp.Open();
                    sp.WriteLine(ssid);
                    sp.WriteLine(password);
                    sp.Close();
                }

                // Send over HTTP
                var content = new StringContent(
                    $"ssid={ssid}&password={password}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync(esp32Url + "/credentials", content);
                string result = await response.Content.ReadAsStringAsync();

                TxtResult.Text = $"Verzonden!\nAntwoord ESP32:\n{result}";
            }
            catch (Exception ex)
            {
                TxtResult.Text = "Fout bij verzenden:\n" + ex.Message;
            }
        }
    }
}
