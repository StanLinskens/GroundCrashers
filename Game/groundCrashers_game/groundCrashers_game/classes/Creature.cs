namespace groundCrashers_game
{

    public class Creature
    {
        public int id { get; set; }
        public string name { get; set; }
        public string primary_type { get; set; }
        public string subtype { get; set; }
        public Stats stats { get; set; }
        public string element { get; set; }
        public string ability { get; set; }
        public string description { get; set; }
        public bool alive { get; set; } = true;
    }

    public class Stats
    {
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int speed { get; set; }
    }

}