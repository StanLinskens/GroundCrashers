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

            [Elements.Ice] = new Element
            {
                Name = Elements.Ice,
                StrongAgainst = new List<Elements> { Elements.Nature, Elements.Water },
                WeakAgainst = new List<Elements> { Elements.Fire, Elements.Earth }
            },

            [Elements.Toxic] = new Element
            {
                Name = Elements.Toxic,
                StrongAgainst = new List<Elements> { Elements.Nature },
                WeakAgainst = new List<Elements> { Elements.Earth, Elements.Wind }
            },

            [Elements.Fire] = new Element
            {
                Name = Elements.Fire,
                StrongAgainst = new List<Elements> { Elements.Nature, Elements.Ice },
                WeakAgainst = new List<Elements> { Elements.Water, Elements.Earth }
            },

            [Elements.Water] = new Element
            {
                Name = Elements.Water,
                StrongAgainst = new List<Elements> { Elements.Fire, Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Nature, Elements.Electric }
            },

            [Elements.Draconic] = new Element
            {
                Name = Elements.Draconic,
                StrongAgainst = new List<Elements> { Elements.Psychic, Elements.Light },
                WeakAgainst = new List<Elements> { Elements.Earth, Elements.Wind }
            },

            [Elements.Earth] = new Element
            {
                Name = Elements.Earth,
                StrongAgainst = new List<Elements> { Elements.Electric, Elements.Fire },
                WeakAgainst = new List<Elements> { Elements.Water, Elements.Nature }
            },

            [Elements.Dark] = new Element
            {
                Name = Elements.Dark,
                StrongAgainst = new List<Elements> { Elements.Light, Elements.Psychic },
                WeakAgainst = new List<Elements> { Elements.Light, Elements.Demonic }
            },

            [Elements.Wind] = new Element
            {
                Name = Elements.Wind,
                StrongAgainst = new List<Elements> { Elements.Toxic, Elements.Electric },
                WeakAgainst = new List<Elements> { Elements.Ice, Elements.Earth }
            },

            [Elements.Psychic] = new Element
            {
                Name = Elements.Psychic,
                StrongAgainst = new List<Elements> { Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Ice, Elements.Psychic }
            },

            [Elements.Light] = new Element
            {
                Name = Elements.Light,
                StrongAgainst = new List<Elements> { Elements.Dark, Elements.Demonic },
                WeakAgainst = new List<Elements> { Elements.Dark, Elements.Magnetic }
            },

            [Elements.Demonic] = new Element
            {
                Name = Elements.Demonic,
                StrongAgainst = new List<Elements> { Elements.Psychic, Elements.Draconic },
                WeakAgainst = new List<Elements> { Elements.Light, Elements.Electric }
            },

            [Elements.Electric] = new Element
            {
                Name = Elements.Electric,
                StrongAgainst = new List<Elements> { Elements.Water, Elements.Magnetic },
                WeakAgainst = new List<Elements> { Elements.Earth, Elements.Wind }
            },

            [Elements.Acid] = new Element
            {
                Name = Elements.Acid,
                StrongAgainst = new List<Elements> { Elements.Earth },
                WeakAgainst = new List<Elements> { Elements.Water, Elements.Psychic }
            },

            [Elements.Magnetic] = new Element
            {
                Name = Elements.Magnetic,
                StrongAgainst = new List<Elements> { Elements.Electric },
                WeakAgainst = new List<Elements> { Elements.Acid, Elements.Earth }
            },

            [Elements.ALL] = new Element
            {
                Name = Elements.ALL,
                StrongAgainst = new List<Elements> {  },
                WeakAgainst = new List<Elements> {  }
            },

            [Elements.none] = new Element
            {
                Name = Elements.none,
                StrongAgainst = new List<Elements> {  },
                WeakAgainst = new List<Elements> {  }
            },

        };

        public static float GetElementEffectiveness(Elements attacker, Elements defender)
        {
            if (Chart[attacker].StrongAgainst.Contains(defender)) return 1.5f;
            else if (Chart[attacker].WeakAgainst.Contains(defender)) return 0.5f;
            return 1.0f;
        }
    }
}
