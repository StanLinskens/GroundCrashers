using groundCrashers_game.classes;
using System.Xml.Linq;

namespace groundCrashers_game
{
    public enum Primaries
    {
        Verdant,
        Primal,
        Apex,
        Sapient,
        Synthetic,
        God,
        Titan
    }

    public class Primary
    {
        public Primaries Name { get; set; }
        public List<Primaries> StrongAgainst { get; set; } = new();
        public List<Primaries> BetterAgainst { get; set; } = new();
        public List<Primaries> WeakerAgainst { get; set; } = new();
        public List<Primaries> WeakAgainst { get; set; } = new();
    }

    public static class PrimaryChart
    {
        public static readonly Dictionary<Primaries, Primary> Chart = new()
        {
            [Primaries.Verdant] = new Primary
            {
                Name = Primaries.Verdant,
                StrongAgainst = new List<Primaries> { Primaries.Synthetic },
                BetterAgainst = new List<Primaries> { Primaries.Sapient },
                WeakerAgainst = new List<Primaries> { Primaries.Apex },
                WeakAgainst = new List<Primaries> { Primaries.Primal }
            },

            [Primaries.Primal] = new Primary
            {
                Name = Primaries.Primal,
                StrongAgainst = new List<Primaries> { Primaries.Verdant },
                BetterAgainst = new List<Primaries> { Primaries.Synthetic },
                WeakerAgainst = new List<Primaries> { Primaries.Sapient },
                WeakAgainst = new List<Primaries> { Primaries.Apex }
            },

            [Primaries.Apex] = new Primary
            {
                Name = Primaries.Apex,
                StrongAgainst = new List<Primaries> { Primaries.Primal },
                BetterAgainst = new List<Primaries> { Primaries.Verdant },
                WeakerAgainst = new List<Primaries> { Primaries.Synthetic },
                WeakAgainst = new List<Primaries> { Primaries.Sapient }
            },

            [Primaries.Sapient] = new Primary
            {
                Name = Primaries.Sapient,
                StrongAgainst = new List<Primaries> { Primaries.Apex },
                BetterAgainst = new List<Primaries> { Primaries.Primal },
                WeakerAgainst = new List<Primaries> { Primaries.Verdant },
                WeakAgainst = new List<Primaries> { Primaries.Synthetic }
            },

            [Primaries.Synthetic] = new Primary
            {
                Name = Primaries.Synthetic,
                StrongAgainst = new List<Primaries> { Primaries.Sapient },
                BetterAgainst = new List<Primaries> { Primaries.Apex },
                WeakerAgainst = new List<Primaries> { Primaries.Primal },
                WeakAgainst = new List<Primaries> { Primaries.Verdant }
            },

            [Primaries.God] = new Primary
            {
                Name = Primaries.God,
                StrongAgainst = new List<Primaries> { Primaries.Titan },
                BetterAgainst = new List<Primaries> { Primaries.Verdant, Primaries.Primal, Primaries.Apex, Primaries.Sapient, Primaries.Synthetic },
                WeakerAgainst = new List<Primaries> { Primaries.God },
                WeakAgainst = new List<Primaries> { Primaries.Titan }
            },

            [Primaries.Titan] = new Primary
            {
                Name = Primaries.Titan,
                StrongAgainst = new List<Primaries> { Primaries.God },
                BetterAgainst = new List<Primaries> { Primaries.Sapient },
                WeakerAgainst = new List<Primaries> { Primaries.God },
                WeakAgainst = new List<Primaries> { Primaries.Titan }
            }
        };

        public static float GetEffectiveness(Primaries attacker, Primaries defender)
        {
            if (Chart[attacker].StrongAgainst.Contains(defender)) return 1.5f;
            else if (Chart[attacker].BetterAgainst.Contains(defender)) return 1.25f;
            else if (Chart[attacker].WeakerAgainst.Contains(defender)) return 0.75f;
            else if (Chart[attacker].WeakAgainst.Contains(defender)) return 0.5f;
            return 1.0f;
        }
    }
}
