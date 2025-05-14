using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game.classes
{
    class manager
    {
        public int damage(int attack, int Defence)
        { 
            float damage = attack / 100f * (100f - ((100f / 600f) * Defence));
            int roundedDamage = (int)Math.Round(damage);

            return roundedDamage;
        }
    }
}
