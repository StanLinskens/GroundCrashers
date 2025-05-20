using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using groundCrashers_game;
using System.Text;
using System.Windows;

namespace groundCrashers_game.classes
{
    public class Manager
    {
        public List<Creature> AllCreatures { get; private set; } = new();
        public List<Actor> CurrentActors { get; private set; } = new();

        public Creature? ActivePlayerCreature { get; private set; }
        public Creature? ActiveCpuCreature { get; private set; }

        private static readonly Random _rnd = new Random();

        public enum ActionType
        {
            Attack,
            ElementAttack,
            Defend,
            Swap
        }

        // Index into CurrentActors: 0 = player, 1 = CPU
        private int _currentActorIndex = 0;

        // Round counter (increments after both actors have taken a turn)
        public int RoundNumber { get; private set; } = 1;

        public static Daytimes GetRandomDaytime()
        {
            var values = Enum.GetValues(typeof(Daytimes));
            int index = _rnd.Next(values.Length);
            return (Daytimes)values.GetValue(index);
        }

        public static Biomes GetRandomBiome()
        {
            var values = Enum.GetValues(typeof(Biomes));
            int index = _rnd.Next(values.Length);
            return (Biomes)values.GetValue(index);
        }

        public static Weathers GetRandomWeather()
        {
            var values = Enum.GetValues(typeof(Weathers));
            int index = _rnd.Next(values.Length);
            return (Weathers)values.GetValue(index);
        }

        // Damage formula
        public int Damage(int attack, int defense)
        {
            float damage = attack / 100f * (100f - ((100f / 600f) * defense));
            return (int)Math.Round(damage);
        }

        /// <summary>
        /// Processes one actor’s action.  After resolving that action, it advances to the next actor.
        /// Once both actors have gone, it increments RoundNumber.
        /// </summary>
        /// <param name="action">
        ///   Which action this actor chose (Attack, ElementAttack, Defend, or Swap).
        /// </param>
        /// <param name="swapIndex">
        ///   If action == Swap, this is the index (0-based) of the creature in that actor’s list
        ///   to swap in as the new ActiveCreature.  (Ignore otherwise.)
        /// </param>
        public void ProcessTurn(ActionType action, int swapIndex = -1)
        {
            // 1) Identify attacker and defender based on _currentActorIndex
            Actor currentActor = CurrentActors[_currentActorIndex];
            Actor otherActor = CurrentActors[1 - _currentActorIndex];

            // Pick the right “active” creature fields:
            Creature? attacker = (currentActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            Creature? defender = (otherActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            if (attacker == null || defender == null)
            {
                throw new InvalidOperationException("One of the active creatures is null.");
            }

            // 2) Resolve the chosen action
            switch (action)
            {
                case ActionType.Attack:
                    {
                        // Simple physical attack
                        int damageDealt = Damage(attacker.stats.attack, defender.stats.defense);
                        defender.stats.hp -= damageDealt;
                        if (defender.stats.hp < 0) defender.stats.hp = 0;

                        // (You can replace MessageBox.Show with your own UI‐update calls later)
                        MessageBox.Show(
                            $"{attacker.name} attacks {defender.name} for {damageDealt} damage.\n" +
                            $"{defender.name} now has {defender.stats.hp} HP."
                        );
                        break;
                    }

                case ActionType.ElementAttack:
                    {
                        // Placeholder: insert your elemental damage formula here
                        // e.g. int elementDamage = CalculateElementDamage(attacker, defender);
                        // defender.health -= elementDamage; etc.

                        MessageBox.Show("ElementAttack chosen! (Implement your elemental logic here.)");
                        break;
                    }

                case ActionType.Defend:
                    {
                        // Placeholder: grant a “defense buff” or status in your Creature class
                        // e.g. attacker.IsDefending = true;

                        MessageBox.Show($"{attacker.name} is defending this turn!");
                        break;
                    }

                case ActionType.Swap:
                    {
                        // Make sure swapIndex is valid
                        if (swapIndex < 0 || swapIndex >= currentActor.Creatures.Count)
                        {
                            MessageBox.Show("Invalid swap index.");
                            break;
                        }

                        // Swap-in the chosen creature
                        Creature chosen = currentActor.Creatures[swapIndex];
                        if (currentActor.IsPlayer)
                        {
                            ActivePlayerCreature = chosen;
                            MessageBox.Show($"Player swapped to {chosen.name}!");
                        }
                        else
                        {
                            ActiveCpuCreature = chosen;
                            MessageBox.Show($"CPU swapped to {chosen.name}!");
                        }

                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), "Unknown action type.");
            }

            // 3) Check if the defender (or attacker) fainted, etc.
            //    You can put win/lose logic here:
            //
            //    if (ActivePlayerCreature.health <= 0) { /* player loses or auto‐switch */ }
            //    if (ActiveCpuCreature.health <= 0)    { /* CPU loses or auto‐switch */ }

            // 4) Advance to the next actor’s turn
            _currentActorIndex = 1 - _currentActorIndex;

            // 5) If we’ve just returned to actor 0, that means both have acted → increment round
            if (_currentActorIndex == 0)
            {
                RoundNumber++;
                // (Optional) Notify UI that a new round has started:
                // e.g. OnRoundAdvanced?.Invoke(RoundNumber);
            }
        }


        // Load all creatures once (e.g., when game starts)
        public void LoadAllCreatures()
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "JsonData", "creatures.json");
            if (!File.Exists(path)) throw new FileNotFoundException("Creatures JSON not found.");

            var text = File.ReadAllText(path);
            AllCreatures = JsonConvert.DeserializeObject<List<Creature>>(text) ?? new List<Creature>();
        }

        // Load actors from a level file (player and CPU)
        public void LoadActorsForLevel(string levelFileName)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "JsonData", "Levels", levelFileName);
            if (!File.Exists(path)) throw new FileNotFoundException("Level JSON not found.");

            var text = File.ReadAllText(path);
            var levelData = JsonConvert.DeserializeObject<LevelConfig>(text) ?? throw new Exception("Invalid level config");

            CurrentActors.Clear();
            foreach (var ac in levelData.Actors)
            {
                var actor = new Actor(ac.Name, ac.IsPlayer);
                foreach (int creatureId in ac.CreatureIds)
                {
                    var creature = AllCreatures.Find(c => c.id == creatureId);
                    if (creature != null)
                        actor.Creatures.Add(creature);
                }
                CurrentActors.Add(actor);
            }
        }

        public void AddPlayerCreatures(Creature newCreature)
        {

            // Create Player Actor
            Actor playerActor = GetPlayerActor();
            if(playerActor == null)
            {
                MessageBox.Show("fuck this shit");
            }
            else
            {
                playerActor.Creatures.Add(newCreature);
            }
        }

        public void LoadActorsForBattleMode()
        {
            CurrentActors.Clear();

            // Create CPU Actor
            var cpuActor = new Actor("CPU", false);
            cpuActor.Creatures.AddRange(GetRandomCreatures(3));
            CurrentActors.Add(cpuActor);

            var playerActor = new Actor("Player", true);
            CurrentActors.Add(playerActor);


            // ←─── After you have given each actor its list of 3 creatures,
            // pick the “first” one to be the active one on‐screen:
            ActiveCpuCreature = cpuActor.Creatures.Count > 0
                ? cpuActor.Creatures[0]
                : null;
        }

        private List<Creature> GetRandomCreatures(int count)
        {
            var randomCreatures = new List<Creature>();
            var available = new List<Creature>(AllCreatures); // Make a copy so we can remove

            for (int i = 0; i < count && available.Count > 0; i++)
            {
                int index = _rnd.Next(available.Count);
                randomCreatures.Add(available[index]);
                available.RemoveAt(index); // Avoid duplicates
            }

            return randomCreatures;
        }
        public void PrintActors()
        {
            StringBuilder message = new StringBuilder();

            foreach (var actor in CurrentActors)
            {
                message.AppendLine($"=== {actor.Name} === (IsPlayer: {actor.IsPlayer})");

                if (actor.Creatures.Count == 0)
                {
                    message.AppendLine("  No creatures assigned.");
                }
                else
                {
                    for (int i = 0; i < actor.Creatures.Count; i++)
                    {
                        var creature = actor.Creatures[i];
                        message.AppendLine($"  {i + 1}. {creature.name} (ID: {creature.id})");
                    }
                }

                message.AppendLine(); // Empty line between actors
            }

            MessageBox.Show(message.ToString(), "Actor Info");
        }

        public Actor GetPlayerActor() => CurrentActors.Find(a => a.IsPlayer);
        public Actor GetCpuActor() => CurrentActors.Find(a => !a.IsPlayer);
    }

    // Support class for level structure
    public class LevelConfig
    {
        public int Level { get; set; }
        public List<ActorConfig> Actors { get; set; } = new();
    }

    public class ActorConfig
    {
        public string Name { get; set; }
        public bool IsPlayer { get; set; }
        public List<int> CreatureIds { get; set; } = new();
    }
}
