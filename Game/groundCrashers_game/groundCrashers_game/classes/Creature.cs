using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game
{

    //class Creature
    //{
    //    int Id { get; set; }
    //    string Name { get; set; }
    //    string PrimaryType { get; set; }
    //    string Subtype { get; set; }
    //    Stats Stats { get; set; }
    //    string Element { get; set; }
    //    string Ability { get; set; }
    //    string Description { get; set; }

    //    Creature(
    //        int id,
    //        string name,
    //        string primaryType,
    //        string subtype,
    //        Stats stats,
    //        string element,
    //        string ability,
    //        string description,
    //        string scientificName)
    //    {
    //        Id = id;
    //        Name = name;
    //        PrimaryType = primaryType;
    //        Subtype = subtype;
    //        Stats = stats;
    //        Element = element;
    //        Ability = ability;
    //        Description = description;
    //    }
    //}

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
        public string scientific_name { get; set; }
    }

    public class Stats
    {
        public int hp { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int speed { get; set; }
    }

}