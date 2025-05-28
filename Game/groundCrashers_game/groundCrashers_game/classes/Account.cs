using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    public class Account
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public int current_biome_id { get; set; }
        public int current_biome_lvl_id { get; set; }
    }
    public class AccountManager
    {
        private static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "biomes.json");

        public static List<Account> LoadAccounts()
        {
            if (!File.Exists(FilePath))
                return new List<Account>();

            var json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<Account>>(json) ?? new List<Account>();
        }

        public static void SaveAccounts(List<Account> accounts)
        {
            var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public static void AddAccount(string username, string password)
        {
            var accounts = LoadAccounts();

            // Generate new ID
            int newId = accounts.Any() ? accounts.Max(a => a.id) + 1 : 1;

            accounts.Add(new Account
            {
                id = newId,
                name = username,
                password = password,
                current_biome_id = 0,
                current_biome_lvl_id = 1
            });

            SaveAccounts(accounts);
        }
    }
}
