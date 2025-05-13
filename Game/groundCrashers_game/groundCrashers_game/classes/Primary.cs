using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game
{
    enum Primaries
    {
        Verdant,
        Primal,
        Apex,
        Sapient,
        Synthetic,
        God,
        Titan
    }

    class Primary
    {
        string Name { get; set; }
        List<string> StrongAgainst { get; set; }
        List<string> WeakAgainst { get; set; }
    }

}
