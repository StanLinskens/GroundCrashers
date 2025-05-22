using Newtonsoft.Json;
using System;
using System.Diagnostics.Eventing.Reader;
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

        public void ProcessTurn(ActionType action, int swapIndex = -1)
        {
            // 1) get Enemy action
            bool cpuDied = false;

            if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
            {
                _currentActorIndex = 0;
            }
            else
            {
                _currentActorIndex = 1;
            }

            ActionChoice(action);
            cpuDied = DeadCheck(cpuDied);
            ActionChoice(action);
            cpuDied = DeadCheck(cpuDied);

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

        private void ActionChoice(ActionType action)
        {
            // 2) Identify attacker and defender based on _currentActorIndex
            Actor currentActor = CurrentActors[_currentActorIndex];
            Actor otherActor = CurrentActors[1 - _currentActorIndex];

            // Pick the right “active” creature fields:
            Creature? attacker = (currentActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            Creature? defender = (otherActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            if(attacker != null && defender != null)
            {
                if (_currentActorIndex == 0)
                {
                    action = CpuAction();
                }
                // find out curse effect
                string curse = CurseEffect(attacker);

                switch (action)
                {
                    case ActionType.Attack:
                        {
                            int DamageDealt = Damage(attacker.stats.attack, defender.stats.defense);

                            if (curse == "SelfHit")
                            {
                                attacker.stats.hp -= DamageDealt;
                            }
                            else if (curse != "missed" || curse != "SkipTurn")
                            {
                                defender.stats.hp -= DamageDealt;
                            }
                            break;
                        }

                    case ActionType.ElementAttack:
                        {
                            int DamageDealt = Damage(attacker.stats.attack, defender.stats.defense);
                            DamageDealt = (int)Math.Round(DamageDealt * 0.4f); //less damage because elemental

                            if (curse == "SelfHit")
                            {
                                attacker.stats.hp -= DamageDealt;
                                attacker.curse = attacker.element;
                            }
                            else if (curse != "missed" || curse != "SkipTurn")
                            {
                                defender.stats.hp -= DamageDealt;
                                defender.curse = attacker.element;
                            }

                            break;
                        }

                    case ActionType.Defend:
                        {
                            attacker.stats.hp += attacker.stats.max_hp / 25; // 25% heal
                            attacker.curse = "none";
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

                if ((attacker != null) && attacker.curse == "none")
                {
                    attacker.stats.attack = attacker.stats.max_attack;
                    attacker.stats.defense = attacker.stats.max_defense;
                    attacker.stats.speed = attacker.stats.max_speed;
                }

                if ((attacker != null) && attacker.curse == "ALL")
                {
                    RandomCurse(attacker);
                }
            }
        }

        private void RandomCurse(Creature ActiveCreature)
        {
            int randomnumber = _rnd.Next(1, 16);

            if (randomnumber == 1) { ActiveCreature.curse = "Nature"; }
            if (randomnumber == 2) { ActiveCreature.curse = "Ice"; }
            if (randomnumber == 3) { ActiveCreature.curse = "Toxic"; }
            if (randomnumber == 4) { ActiveCreature.curse = "Fire"; }
            if (randomnumber == 5) { ActiveCreature.curse = "Water"; }
            if (randomnumber == 6) { ActiveCreature.curse = "Draconic"; }
            if (randomnumber == 7) { ActiveCreature.curse = "Earth"; }
            if (randomnumber == 8) { ActiveCreature.curse = "Dark"; }
            if (randomnumber == 9) { ActiveCreature.curse = "Wind"; }
            if (randomnumber == 10) { ActiveCreature.curse = "Psychic"; }
            if (randomnumber == 11) { ActiveCreature.curse = "Light"; }
            if (randomnumber == 12) { ActiveCreature.curse = "Electric"; }
            if (randomnumber == 13) { ActiveCreature.curse = "Acid"; }
            if (randomnumber == 14) { ActiveCreature.curse = "Magnetic"; }
            if (randomnumber == 15) { ActiveCreature.curse = "Demonic"; }
        }

        private string CurseEffect(Creature activeCreature)
        {
            int randomNumber = _rnd.Next(100);
            if (activeCreature.curse != "none")
            {
                activeCreature.stats.attack = activeCreature.stats.max_attack;
                activeCreature.stats.speed = activeCreature.stats.speed;

                if (activeCreature.curse == "Nature")
                {
                    activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.65f); // 35% speed reduction 
                }
                else if (activeCreature.curse == "Ice")
                {
                    activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.05f); // 5% max hp damage
                    activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.9f); // 10% attack reduction
                }
                else if (activeCreature.curse == "Toxic" || activeCreature.curse == "Fire")
                {
                    activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.1f); // 5% max hp damage
                }
                else if (activeCreature.curse == "Water")
                {
                    activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.85f); // 15% attack reduction
                    activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.85f); // 15% speed reduction 
                }
                else if (activeCreature.curse == "Draconic" || activeCreature.curse == "Light" || activeCreature.curse == "Dark")
                {
                    return randomNumber < 20 ? "missed" : "none";
                }
                else if (activeCreature.curse == "Earth")
                {
                    activeCreature.stats.defense = activeCreature.stats.max_defense;
                    activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * 0.70f); // 30% defense reduction
                }
                else if (activeCreature.curse == "Wind" || activeCreature.curse == "Demonic")
                {
                    return randomNumber < 25 ? "SelfHit" : "none";
                }
                else if (activeCreature.curse == "Psychic")
                {
                    return randomNumber < 20 ? "SkipTurn" : "none";
                }
                else if (activeCreature.curse == "Electric")
                {
                    activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.75f); // 25% speed reduction
                    return randomNumber < 10 ? "SkipTurn" : "none";
                }
                else if (activeCreature.curse == "Acid")
                {
                    activeCreature.stats.defense -= (int)Math.Round(activeCreature.stats.defense * 0.1f); // 10% defence reduction each turn
                }
                else if (activeCreature.curse == "Magnetic")
                {
                    if (randomNumber < 33) 
                    {
                        int save = activeCreature.stats.max_speed;
                        activeCreature.stats.speed = activeCreature.stats.max_defense;
                        activeCreature.stats.defense = activeCreature.stats.max_attack;
                        activeCreature.stats.attack = save;

                    }
                    else if (randomNumber < 66) 
                    {
                        int save = activeCreature.stats.max_attack;
                        activeCreature.stats.attack = activeCreature.stats.max_speed;
                        activeCreature.stats.speed = activeCreature.stats.max_defense;
                        activeCreature.stats.defense = save;
                    }
                    else 
                    {
                        int save = activeCreature.stats.defense;
                        activeCreature.stats.defense = activeCreature.stats.max_attack;
                        activeCreature.stats.attack = activeCreature.stats.max_speed;
                        activeCreature.stats.speed = save;
                    }
                }


            }


            return "none";
        }
        private ActionType CpuAction()
        {
            int randomNumber = _rnd.Next(100);

            int cpuHp = ActiveCpuCreature.stats.hp; 
            int cpuMaxHp = ActiveCpuCreature.stats.max_hp;
            int cpuPercentage = (int)Math.Round((float)cpuHp / cpuMaxHp * 100);

            int playerHp = ActivePlayerCreature.stats.hp;
            int playerMaxHp = ActivePlayerCreature.stats.max_hp;
            int playerPercentage = (int)Math.Round((float)playerHp / playerMaxHp * 100);

            if (ActiveCpuCreature.curse != "none")
            {
                if (cpuPercentage > 35)
                {
                    return randomNumber < 33 ? ActionType.Attack : ActionType.Defend; // 33/66
                }
                else if (playerPercentage > 35)
                {
                    if (randomNumber < 20) return ActionType.Defend;
                    else if (randomNumber < 55) return ActionType.Attack; // 35% window (20–54)
                    else return ActionType.ElementAttack; // 45%
                }
                else
                {
                    return randomNumber < 25 ? ActionType.Defend : ActionType.Attack;
                }
            }
            if (ActivePlayerCreature.curse != "none")
            {
                return ActionType.Attack;
            }
            if (ActivePlayerCreature.curse == "none")
            {
                if (cpuPercentage > 85)
                {
                    if (playerPercentage > 65)
                    {
                        return randomNumber < 33 ? ActionType.Attack : ActionType.ElementAttack; // 33/66
                    }
                    else
                    {
                        return randomNumber < 15 ? ActionType.ElementAttack : ActionType.Attack; // 15/85
                    }
                }
                if (cpuPercentage > 45)
                {
                    if (playerPercentage > 55)
                    {
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 45) return ActionType.Attack; // 35% window (10–44)
                        else return ActionType.ElementAttack; // 55%
                    }
                    else
                    {
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 25) return ActionType.ElementAttack; // 15% window (10–24)
                        else return ActionType.Attack; // 75%
                    }
                }
                else
                {
                    if (playerPercentage > 55)
                    {
                        return randomNumber < 25 ? ActionType.Attack : ActionType.ElementAttack;
                    }
                    else
                    {
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 35) return ActionType.ElementAttack; // 25% window (10–34)
                        else return ActionType.Attack; // 65%
                    }
                }
            }
            return ActionType.Attack; // Default action
        }

        private bool DeadCheck(bool cpuDied)
        {
            if ((ActivePlayerCreature != null) && ActivePlayerCreature.stats.hp <= 0)
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

            if ((ActiveCpuCreature != null) && ActiveCpuCreature.stats.hp <= 0)
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
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            if (!File.Exists(path)) throw new FileNotFoundException("Creatures JSON not found.");

            var text = File.ReadAllText(path);
            AllCreatures = JsonConvert.DeserializeObject<List<Creature>>(text) ?? new List<Creature>();
        }

        // Load actors from a level file (player and CPU)
        public void LoadActorsForLevel(string levelFileName)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "Levels", levelFileName);
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
