using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace groundCrashers_game
{

    class Creature
    {
        int Id { get; set; }
        string Name { get; set; }
        string PrimaryType { get; set; }
        string Subtype { get; set; }
        Stats Stats { get; set; }
        string Element { get; set; }
        string Ability { get; set; }
        string Description { get; set; }
        string ScientificName { get; set; }

        Creature(
            int id,
            string name,
            string primaryType,
            string subtype,
            Stats stats,
            string element,
            string ability,
            string description,
            string scientificName)
        {
            Id = id;
            Name = name;
            PrimaryType = primaryType;
            Subtype = subtype;
            Stats = stats;
            Element = element;
            Ability = ability;
            Description = description;
            ScientificName = scientificName;
        }
    }
}