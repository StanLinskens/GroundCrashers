using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    class manager
    {
        public int damage_To_Cpu()
        { 
            int De_C = 75;
            int attack_P = 85;
            float defence_C = (100f / 600f) * De_C;
            float damage_To_C = attack_P / 100f * (100f - defence_C);
            int roundedDamage_To_C = (int)Math.Round(damage_To_C);

            return roundedDamage_To_C;
        }

        public int damage_To_Player()
        {
            int De_P = 150;
            int attack_C = 150;
            float defence_P = (100f / 600f) * De_P;
            float damage_To_P = attack_C / 100f * (100f - defence_P);
            int roundedDamage_To_P = (int)Math.Round(damage_To_P);

            return roundedDamage_To_P;
        }
    }
}
