using System;
using System.Collections.Generic;

namespace groundCrashers_game.classes
{
    public class Actor
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<Creature> Creatures { get; set; } = new List<Creature>();

        public bool IsPlayer { get; set; } // True = player, False = computer

        public Actor(string name, bool isPlayer)
        {
            Name = name;
            Id = Guid.NewGuid();
            IsPlayer = isPlayer;
        }
    }
}
