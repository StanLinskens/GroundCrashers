using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    enum SubTypes
    {
        Aquatic,
        Terrestrial,
        avian
    }

    class SubType
    {
        string Name { get; set; }
        string PassiveBoost { get; set; }
    }

}
