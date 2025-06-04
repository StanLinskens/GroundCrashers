using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace groundCrashers_game.classes
{
    public class Account
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int current_biome_id { get; set; }
        public int current_biome_lvl_id { get; set; }
        public int LVL { get; set; }
        public int XP { get; set; }
        public int coins { get; set; }
    }

    public class ActiveAccount
    {
        public static int Active_id { get; set; } = 0;
        public static string Active_name { get; set; }
        public static string Active_password { get; set; }
        public static int Active_current_biome_id { get; set; }
        public static int Active_current_biome_lvl_id { get; set; }
        public static int Active_LVL { get; set; }
        public static int Active_XP { get; set; }
        public static int Active_coins { get; set; }
    }
    public class AccountManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "accounts.json");

        public static bool Login(string username, string password)
        {
            var accounts = LoadAccounts();

            var account = accounts.FirstOrDefault(a =>
                a.name.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                a.password == password);

            if (account != null)
            {
                ActiveAccount.Active_id = account.id;
                ActiveAccount.Active_name = account.name;
                ActiveAccount.Active_password = account.password;
                ActiveAccount.Active_current_biome_id = account.current_biome_id;
                ActiveAccount.Active_current_biome_lvl_id = account.current_biome_lvl_id;
                ActiveAccount.Active_LVL = account.LVL;
                ActiveAccount.Active_XP = account.XP;
                ActiveAccount.Active_coins = account.coins;

                return true;
            }

            return false;
        }
        public static List<Account> LoadAccounts()
        {
            if (!File.Exists(FilePath))
                return new List<Account>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
        }

        public static void UpdateActiveAccount()
        {
            var accounts = LoadAccounts();

            var account = accounts.FirstOrDefault(a => a.id == ActiveAccount.Active_id);
            if (account != null)
            {
                account.current_biome_id = ActiveAccount.Active_current_biome_id;
                account.current_biome_lvl_id = ActiveAccount.Active_current_biome_lvl_id;
                account.LVL = ActiveAccount.Active_LVL;
                account.XP = ActiveAccount.Active_XP;
                account.coins = ActiveAccount.Active_coins;
                // Optionally update name or password if those change
                SaveAccounts(accounts);
            }
        }

        public static void SaveAccounts(List<Account> accounts)
        {
            var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void AddAccount(string username, string password)
        {
            var accounts = LoadAccounts();

            // Check if the username already exists (case-insensitive)
            if (accounts.Any(a => a.name.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("An account with that username already exists.", "Duplicate Username", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Generate new ID
            int newId = accounts.Any() ? accounts.Max(a => a.id) + 1 : 1;

            accounts.Add(new Account
            {
                id = newId,
                name = username,
                password = password,
                current_biome_id = 0,
                current_biome_lvl_id = 1,
                LVL = 0,
                XP = 0,
                coins = 0
            });

            MessageBox.Show("Account created");
            SaveAccounts(accounts);
        }
    }
}
