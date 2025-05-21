using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

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
            bool cpuDied = false;
            // 2) Resolve the chosen action
            switch (action)
            {
                case ActionType.Attack:
                    {
                        if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
                        {
                            int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                            ActivePlayerCreature.stats.hp -= DamageDealt;

                            MessageBox.Show(
                            $"{ActiveCpuCreature.name} attacks {ActivePlayerCreature.name} for {DamageDealt} damage.\n" +
                            $"{ActivePlayerCreature.name} now has {ActivePlayerCreature.stats.hp} HP.");
                            cpuDied = DeadCheck();

                            if(ActivePlayerCreature != null && ActiveCpuCreature != null)
                            {
                                DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                                ActiveCpuCreature.stats.hp -= DamageDealt;

                                MessageBox.Show(
                                $"{ActivePlayerCreature.name} attacks {ActiveCpuCreature.name} for {DamageDealt} damage.\n" +
                                $"{ActiveCpuCreature.name} now has {ActiveCpuCreature.stats.hp} HP.");
                                cpuDied = DeadCheck();
                            }
                        }
                        else
                        {
                            int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                            ActiveCpuCreature.stats.hp -= DamageDealt;

                            MessageBox.Show(
                            $"{ActivePlayerCreature.name} attacks {ActiveCpuCreature.name} for {DamageDealt} damage.\n" +
                            $"{ActiveCpuCreature.name} now has {ActiveCpuCreature.stats.hp} HP.");

                            cpuDied = DeadCheck();

                            if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                            {
                                DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                                ActivePlayerCreature.stats.hp -= DamageDealt;

                                MessageBox.Show(
                                $"{ActiveCpuCreature.name} attacks {ActivePlayerCreature.name} for {DamageDealt} damage.\n" +
                                $"{ActivePlayerCreature.name} now has {ActivePlayerCreature.stats.hp} HP.");
                                cpuDied = DeadCheck();
                            }

                        }
                        CpuAction();
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
                        MessageBox.Show("block chosen! (Implement your elemental logic here.)");
                        break;
                    }
                case ActionType.Swap:
                    {
                        break;
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(action), "Unknown action type.");
            }


            // 4) Advance to the next actor’s turn
            _currentActorIndex = 1 - _currentActorIndex;

            // 5) If we’ve just returned to actor 0, that means both have acted → increment round
            if (_currentActorIndex == 0)
            {
                RoundNumber++;
                // (Optional) Notify UI that a new round has started:
                // e.g. OnRoundAdvanced?.Invoke(RoundNumber);
            }
            if (cpuDied == true)
            {
                Actor cpuActor = GetCpuActor();
                ActiveCpuCreature = cpuActor.Creatures.FirstOrDefault(c => c.alive == true);
            }

            if (ActiveCpuCreature == null)
            {
                MessageBox.Show("You win!");
            }
        }

        private string CpuAction()
        {

            int randomNumber = _rnd.Next(100);

            int cpuHp = ActiveCpuCreature.stats.hp; 
            int cpuMaxHp = ActiveCpuCreature.stats.max_hp;
            int cpuPercentage = (int)Math.Round((float)cpuHp / cpuMaxHp * 100);

            int playerHp = ActivePlayerCreature.stats.hp;
            int playerMaxHp = ActivePlayerCreature.stats.max_hp;
            int playerPercentage = (int)Math.Round((float)playerHp / playerMaxHp * 100);

            if (ActiveCpuCreature.cursed == true)
            {
                if (cpuPercentage > 35)
                {
                    return "Block";
                }
                else if (playerPercentage > 35)
                {
                    if (randomNumber < 20) return "block";
                    else if (randomNumber < 55) return "attack"; // 35% window (20–54)
                    else return "ElementAttack"; // 45%
                }
                else
                {
                    return randomNumber < 25 ? "Block" : "Attack";
                }
            }
            if (cpuPercentage > 85)
            {
                if(playerPercentage > 55)
                {
                    return randomNumber < 25 ? "attack" : "elementattack"; // 25/75
                }
                else
                {
                    return randomNumber < 15 ? "elementattack" : "attack"; // 15/85
                }
            }
            if (cpuPercentage > 45)
            {
                if (playerPercentage > 40)
                {
                    if (randomNumber < 10) return "block";
                    else if (randomNumber < 35) return "attack"; // 25% window (10–34)
                    else return "ElementAttack"; // 65%
                }
                else
                {
                    if (randomNumber < 10) return "block";
                    else if (randomNumber < 35) return "elementattack"; // 25% window (10–34)
                    else return "attack"; // 65%
                }
            }
            else
            {
                if (playerPercentage > 55)
                {
                    return randomNumber < 25 ? "Attack" : "ElementAttack";
                }
                else
                {
                    if (randomNumber < 10) return "block";
                    else if (randomNumber < 35) return "elementattack"; // 25% window (10–34)
                    else return "attack"; // 65%
                }
            }
        }

        private bool DeadCheck()
        {
            if (ActivePlayerCreature.stats.hp <= 0)
            {
                // Create Player Actor
                Actor playerActor = GetPlayerActor();

                foreach (Creature c in playerActor.Creatures)
                {
                    if (c.name == ActivePlayerCreature.name)
                    {
                        c.alive = false;
                    }
                }

                ActivePlayerCreature = null;
            }

            bool cpuDied = true;
            if (ActiveCpuCreature.stats.hp <= 0)
            {
                // Create CPU Actor
                Actor cpuActor = GetCpuActor();
                foreach (Creature c in cpuActor.Creatures)
                {
                    if (c.name == ActiveCpuCreature.name)
                    {
                        c.alive = false;
                        
                        cpuDied = true;
                    }
                }
                ActiveCpuCreature = null;
            }

            return cpuDied;
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
                        actor.Creatures.Add(creature.Clone());
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
                MessageBox.Show("could not find that GroundCrasher");
            }
            else
            {
                if(playerActor.Creatures.Count < 3)
                {
                    foreach (var creature in playerActor.Creatures)
                    {
                        if (creature.id == newCreature.id)
                        {
                            MessageBox.Show("You already have that GroundCrasher");
                            return;
                        }
                    }
                    playerActor.Creatures.Add(newCreature.Clone());
                }
                else
                {
                    MessageBox.Show("You can only have 3 GroundCrashers");
                }

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
                var clonedCreature = available[index].Clone();
                randomCreatures.Add(clonedCreature);
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

        public void CurrentPlayerCreatureSet(string name)
        {
            var playerActor = GetPlayerActor();
            if (playerActor != null)
            {
                ActivePlayerCreature = playerActor.Creatures.FirstOrDefault(c => c.name == name);
            }
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
