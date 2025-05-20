namespace groundCrashers_game
{
    enum Elements
    {
        Nature,
        Ice,
        Toxic,
        Fire,
        Warer,
        Dracobnic,
        Earth,
        Dark,
        Wind,
        Psychic,
        Light,
        Demonic,
        Electric,
        Acid,
        Magnetic
    }

    class Element
    {
        string Name { get; set; }
        string StatusEffect { get; set; }
        string EffectDescription { get; set; }
        Effectiveness Effectiveness { get; set; }
    }
}
