namespace groundCrashers_game
{
    public enum Elements
    {
        Nature,
        Ice,
        Toxic,
        Fire,
        Water,
        Draconic,
        Earth,
        Dark,
        Wind,
        Psychic,
        Light,
        Demonic,
        Electric,
        Acid,
        Magnetic,
        ALL,
        none
    }

    public class Element
    {
        public Elements Name { get; set; }
        public List<Elements> StrongAgainst { get; set; } = new();
        public List<Elements> WeakAgainst { get; set; } = new();
    }

    public static class ElementChart
    {
        public static readonly Dictionary<Elements, Element> Chart = new()
        {
            [Elements.Nature] = new Element
            {
                Name = Elements.Nature,
                StrongAgainst = new List<Elements> { Elements.Water, Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Fire, Elements.Toxic }
            },

            [Elements.Nature] = new Element
            {
                Name = Elements.Ice,
                StrongAgainst = new List<Elements> { Elements.Nature, Elements.Water },
                WeakAgainst = new List<Elements> { Elements.Fire, Elements.Earth }
            },

            [Elements.Nature] = new Element
            {
                Name = Elements.Nature,
                StrongAgainst = new List<Elements> { Elements.Water, Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Fire, Elements.Toxic }
            },

            [Elements.Nature] = new Element
            {
                Name = Elements.Nature,
                StrongAgainst = new List<Elements> { Elements.Water, Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Fire, Elements.Toxic }
            },
        };

        public static float GetEffectiveness(Elements attacker, Elements defender)
        {
            if (Chart[attacker].StrongAgainst.Contains(defender)) return 1.5f;
            else if (Chart[attacker].WeakAgainst.Contains(defender)) return 0.5f;
            return 1.0f;
        }
    }
}
