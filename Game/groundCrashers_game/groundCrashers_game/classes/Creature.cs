namespace groundCrashers_game
{

    public class Creature
    {
        public int id { get; set; }
        public string name { get; set; }
        public Primaries primary_type { get; set; }
        public string subtype { get; set; }
        public Stats stats { get; set; }
        public Elements element { get; set; }
        public string ability { get; set; }
        public string description { get; set; }
        public Elements curse { get; set; } = Elements.none;
        public bool alive { get; set; } = true;
        

        public Creature Clone()
        {
            return new Creature
            {
                id = this.id,
                name = this.name,
                primary_type = this.primary_type,
                subtype = this.subtype,
                stats = this.stats?.Clone(),
                element = this.element,
                ability = this.ability,
                description = this.description,
                curse = this.curse,
                alive = this.alive
            };
        }
    }

    public class Stats
    {
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int speed { get; set; }
        public int max_hp { get; set; }
        public int max_attack { get; set; }
        public int max_defense { get; set; }
        public int max_speed { get; set; }

        public Stats Clone()
        {
            int tripledHp = this.hp * 3;

            return new Stats
            {
                hp = tripledHp,
                attack = this.attack,
                defense = this.defense,
                speed = this.speed,
                max_hp = this.max_hp == 0 ? tripledHp : this.max_hp,
                max_attack = this.max_attack == 0 ? this.attack : this.max_attack,
                max_defense = this.max_defense == 0 ? this.defense : this.max_defense,
                max_speed = this.max_speed == 0 ? this.speed : this.max_speed
            };
        }
    }

}