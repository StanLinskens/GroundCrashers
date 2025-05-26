using System;
using System.Net.Http;
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
        }

        private async void BtnGetUid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string response = await client.GetStringAsync(esp32Url);
                TxtResult.Text = response;
            }
            catch (Exception ex)
            {
                TxtResult.Text = "Fout bij verbinden:\n" + ex.Message;
            }
        }
    }
}
