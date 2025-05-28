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
        private readonly Esp32Manager espManager = new Esp32Manager();

        public espConnectionWindow()
        {
            InitializeComponent();
        }

        private async void BtnGetUid_Click(object sender, RoutedEventArgs e)
        {
            //TxtResult.Text = await espManager.GetUidAsync();
        }

        private async void BtnSendcredentials_Click(object sender, RoutedEventArgs e)
        {
            string ssid = txtSSID.Text.Trim();
            string password = txtPASSWORD.Text.Trim();
            //TxtResult.Text = await espManager.SendCredentialsAsync(ssid, password);
        }

    }
}
