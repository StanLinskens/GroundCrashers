using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
                string response = await client.GetStringAsync(esp32Url + "/uid");
                TxtResult.Text = response;
            }
            catch (Exception ex)
            {
                TxtResult.Text = "Fout bij verbinden:\n" + ex.Message;
            }
        }

        private async void BtnSendcredentials_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ssid = txtSSID.Text;
                var password = txtPASSWORD.Text;

                var content = new StringContent(
                    $"ssid={ssid}&password={password}",
                    Encoding.UTF8,
                    "application/x-www-form-urlencoded");

                HttpResponseMessage response = await client.PostAsync(esp32Url + "/credentials", content);
                string result = await response.Content.ReadAsStringAsync();

                TxtResult.Text = $"Send!\nAnwser ESP32:\n{result}";
            }
            catch (Exception ex)
            {
                TxtResult.Text = "error connection:\n" + ex.Message;
            }
        }
    }
}
