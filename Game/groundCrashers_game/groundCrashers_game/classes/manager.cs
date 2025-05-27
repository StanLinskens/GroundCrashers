using DocumentFormat.OpenXml.ExtendedProperties;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq.Expressions;
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

        public List<string> logs { get; set; } = new List<string>();

        private static readonly Random _rnd = new Random();

        private int _maxCreatures = 3;

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
            var selected = (Weathers)values.GetValue(index);
            WeatherChart.currentWeather = selected;
            return selected;
        }

        public void LoadGameData()
        {
            try
            {
                // Load creatures
                LoadAllCreatures();

                // Load elements
                ElementChart.LoadElementsFromJson();

                // Load primaries
                PrimaryChart.LoadPrimariesFromJson();

                // Load weather
                WeatherChart.LoadWeathersFromJson();

                // Load daytime
                DaytimeChart.LoadDaytimesFromJson();

                // load biomes
                BiomeChart.LoadBiomesFromJson();

                logs.Add("Game data loaded successfully");
            }
            catch (Exception ex)
            {
                logs.Add($"Error loading game data: {ex.Message}");
                MessageBox.Show($"Error loading game data: {ex.Message}", "Loading Error");
                throw;
            }
        }

        // Damage formula
        public int Damage(Creature attacker, Creature defender)
        {
            float damage = attacker.stats.attack / 100f * (100f - ((100f / 600f) * defender.stats.defense));
            float multiplier = PrimaryChart.GetPrimaryEffectiveness(attacker.primary_type, defender.primary_type);
            damage *= multiplier;
            logs.Add($"{attacker.name} did {multiplier}x the normal damage");
            return (int)Math.Round(damage);
        }

        public int DamageElemental(Creature attacker, Creature defender)
        {
            float damage = attacker.stats.attack / 100f * (100f - ((100f / 600f) * defender.stats.defense));
            float multiplier = ElementChart.GetElementEffectiveness(attacker.element, defender.element);
            damage *= multiplier;
            damage *= 0.4f; // Elemental attacks do less damage
            defender.curse = attacker.element; // Apply curse based on attacker's element
            logs.Add($"{attacker.name} did {multiplier}x the normal damage");
            return (int)Math.Round(damage);
        }

        public void ProcessTurn(ActionType action, string name = "default")
        {
            // 1) get Enemy action
            ActionType actionCpu = CpuAction();
            bool cpuDied = false;

            if(action == ActionType.Swap)
            {
                _currentActorIndex = 1;
            }
            else if ((actionCpu == ActionType.Swap) || ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
            {
                _currentActorIndex = 0;
            }
            else
            {
                _currentActorIndex = 1;
            }

            ActionChoice(action, actionCpu, name);
            cpuDied = DeadCheck(cpuDied);
            ActionChoice(action, actionCpu, name);
            cpuDied = DeadCheck(cpuDied);

            if (cpuDied == true)
            {
                Actor cpuActor = GetCpuActor();
                ActiveCpuCreature = cpuActor.Creatures.FirstOrDefault(c => c.alive == true);
            }

            if (ActiveCpuCreature == null)
            {
                logs.Add("you win");
                MessageBox.Show("You win!");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }

        private void ActionChoice(ActionType action, ActionType actionCpu, string name)
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
                // find out curse effect
                string curse = CurseEffect(attacker);

                // display action in logs
                if (_currentActorIndex == 0)
                {
                    action = actionCpu;
                    logs.Add("Choice " + attacker.name + " (cpu) " + action.ToString().ToLower());
                }
                else
                {
                    logs.Add("Choice " + attacker.name + " (player) " + action.ToString().ToLower());
                }

                switch (action)
                {
                    case ActionType.Attack:
                        {
                           
                            if (curse == "SelfHit")
                            {
                                int DamageDealt = Damage(attacker, attacker);
                                attacker.stats.hp -= DamageDealt;
                                logs.Add(attacker.name + " hit himself");
                            }
                            else if (curse != "missed" || curse != "SkipTurn")
                            {
                                int DamageDealt = Damage(attacker, defender);
                                defender.stats.hp -= DamageDealt;
                            }
                            else if (curse == "missed")
                            {
                                logs.Add(attacker.name + " missed");
                            }
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                                break;
                        }

                    case ActionType.ElementAttack:
                        {
                            if (curse == "SelfHit")
                            {
                                int DamageDealt = DamageElemental(attacker, attacker);
                                attacker.stats.hp -= DamageDealt;
                                logs.Add(attacker.name + " hit himself");
                                logs.Add(attacker.name + " curse is " + attacker.element.ToString().ToLower());
                            }
                            else if (curse != "missed" || curse != "SkipTurn")
                            {
                                int DamageDealt = DamageElemental(attacker, defender);
                                defender.stats.hp -= DamageDealt;
                                logs.Add(defender.name + " curse is " + attacker.element.ToString().ToLower());
                            }
                            else if (curse == "missed")
                            {
                                logs.Add(attacker.name + " missed");
                            }
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                            break;
                        }

                    case ActionType.Defend:
                        {
                            if (curse != "SkipTurn")
                            {
                                int healing = (int)Math.Round(attacker.stats.max_hp * 0.20f); // 20% heal
                                attacker.stats.hp += healing;
                                if(attacker.stats.hp > attacker.stats.max_hp)
                                {
                                    attacker.stats.hp = attacker.stats.max_hp; // Cap at max hp
                                }
                                attacker.curse = Elements.none;
                            }
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                            break;
                        }
                    case ActionType.Swap:
                        {
                            // if cpu turn
                            if(_currentActorIndex == 0)
                            {
                                Actor cpuActor = GetCpuActor();

                                Creature candidate = cpuActor.Creatures
                                    .Where(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type) >= 1.0f)
                                    .OrderByDescending(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type))
                                    .FirstOrDefault();

                                ActiveCpuCreature = candidate;
                                break;
                            }
                            // if player turn
                            else if (_currentActorIndex == 1)
                            {
                                Actor playerActor = GetPlayerActor();
                                if (name != "default")
                                {
                                    CurrentPlayerCreatureSet(name);
                                    logs.Add("Swapped to " + ActivePlayerCreature.name);
                                }
                                else
                                {
                                    logs.Add("Invalid swap index");
                                }
                                break;
                            }
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

                if ((attacker != null) && attacker.curse == Elements.none)
                {
                    attacker.stats.attack = attacker.stats.max_attack;
                    attacker.stats.defense = attacker.stats.max_defense;
                    attacker.stats.speed = attacker.stats.max_speed;
                }

                if ((attacker != null) && attacker.curse == Elements.ALL)
                {
                    RandomCurse(attacker);
                }
            }
        }

        private void RandomCurse(Creature ActiveCreature)
        {
            int randomnumber = _rnd.Next(1, 16);

            if (randomnumber == 1) { ActiveCreature.curse = Elements.Nature; }
            if (randomnumber == 2) { ActiveCreature.curse = Elements.Ice; }
            if (randomnumber == 3) { ActiveCreature.curse = Elements.Toxic; }
            if (randomnumber == 4) { ActiveCreature.curse = Elements.Fire; }
            if (randomnumber == 5) { ActiveCreature.curse = Elements.Water; }
            if (randomnumber == 6) { ActiveCreature.curse = Elements.Draconic; }
            if (randomnumber == 7) { ActiveCreature.curse = Elements.Earth; }
            if (randomnumber == 8) { ActiveCreature.curse = Elements.Dark; }
            if (randomnumber == 9) { ActiveCreature.curse = Elements.Wind; }
            if (randomnumber == 10) { ActiveCreature.curse = Elements.Psychic; }
            if (randomnumber == 11) { ActiveCreature.curse = Elements.Light; }
            if (randomnumber == 12) { ActiveCreature.curse = Elements.Electric; }
            if (randomnumber == 13) { ActiveCreature.curse = Elements.Acid; }
            if (randomnumber == 14) { ActiveCreature.curse = Elements.Magnetic; }
            if (randomnumber == 15) { ActiveCreature.curse = Elements.Demonic; }
        }

        private string CurseEffect(Creature activeCreature)
        {
            int randomNumber = _rnd.Next(100);
            if (activeCreature.curse != Elements.none)
            {
                activeCreature.stats.attack = activeCreature.stats.max_attack;
                activeCreature.stats.speed = activeCreature.stats.speed;

                switch (activeCreature.curse)
                {
                    case Elements.Nature:
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.60f); // 40% speed reduction 
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        break;
                    case Elements.Ice:
                        {
                            int save = activeCreature.stats.hp;
                            activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.08f); // 8% max hp damage
                            logs.Add(activeCreature.name + " hp went from " + save + " to " + activeCreature.stats.hp);
                            activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.85f); // 15% attack reduction
                            logs.Add(activeCreature.name + " attack went from " + activeCreature.stats.max_attack + " to " + activeCreature.stats.attack);
                            break;
                        }

                    case Elements.Toxic:
                    case Elements.Fire:
                        {
                            int save = activeCreature.stats.hp;
                            activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.15f); // 15% max hp damage
                            logs.Add(activeCreature.name + " hp went from " + save + " to " + activeCreature.stats.hp);
                            break;
                        }

                    case Elements.Water:
                        activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.80f); // 20% attack reduction
                        logs.Add(activeCreature.name + " attack went from " + activeCreature.stats.max_attack + " to " + activeCreature.stats.attack);
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.80f); // 20% speed reduction 
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        break;
                    case Elements.Draconic:
                    case Elements.Light:
                    case Elements.Dark:
                        return randomNumber < 30 ? "missed" : "none";
                    case Elements.Earth:
                        activeCreature.stats.defense = activeCreature.stats.max_defense;
                        activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * 0.70f); // 30% defense reduction
                        logs.Add(activeCreature.name + " defense went from " + activeCreature.stats.max_defense + " to " + activeCreature.stats.defense);
                        break;
                    case Elements.Wind:
                    case Elements.Demonic:
                        return randomNumber < 32 ? "SelfHit" : "none";
                    case Elements.Psychic:
                        return randomNumber < 30 ? "SkipTurn" : "none";
                    case Elements.Electric:
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.70f); // 30% speed reduction
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        return randomNumber < 20 ? "SkipTurn" : "none";
                    case Elements.Acid:
                        activeCreature.stats.defense -= (int)Math.Round(activeCreature.stats.defense * 0.13f); // 13% defence reduction each turn
                        logs.Add(activeCreature.name + " defense went from " + activeCreature.stats.max_defense + " to " + activeCreature.stats.defense);
                        break;
                    case Elements.Magnetic:
                        {
                            int RandomValue = _rnd.Next(1, 201); // 1 to 200
                            if (randomNumber < 33)
                            {
                                activeCreature.stats.defense = activeCreature.stats.max_defense;
                                activeCreature.stats.speed = RandomValue; // Set speed to a random value between 1 and 200
                                logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + RandomValue);
                            }
                            else if (randomNumber < 66)
                            {
                                activeCreature.stats.defense = activeCreature.stats.max_defense;
                                activeCreature.stats.attack = RandomValue; // Set attack to a random value between 1 and 200
                                logs.Add(activeCreature.name + " attack went from " + activeCreature.stats.max_attack + " to " + RandomValue);
                            }
                            else
                            {
                                activeCreature.stats.defense = RandomValue; // Set defense to a random value between 1 and 200
                                logs.Add(activeCreature.name + " defense went from " + activeCreature.stats.max_defense + " to " + RandomValue);
                            }

                            break;
                        }
                }


            }


            return "none";
        }

        private ActionType CpuAction()
        {
            int randomNumber = _rnd.Next(100);

            float multiplier = PrimaryChart.GetPrimaryEffectiveness(ActiveCpuCreature.primary_type, ActivePlayerCreature.primary_type);

            int cpuHp = ActiveCpuCreature.stats.hp; 
            int cpuMaxHp = ActiveCpuCreature.stats.max_hp;
            int cpuPercentage = (int)Math.Round((float)cpuHp / cpuMaxHp * 100);

            int playerHp = ActivePlayerCreature.stats.hp;
            int playerMaxHp = ActivePlayerCreature.stats.max_hp;
            int playerPercentage = (int)Math.Round((float)playerHp / playerMaxHp * 100);

            if ((randomNumber < 33) && multiplier < 1)
            {
                Actor cpuActor = GetCpuActor();
                float currentEffectiveness = PrimaryChart.GetPrimaryEffectiveness(ActiveCpuCreature.primary_type, ActivePlayerCreature.primary_type);

                Creature activeCpuCreatureSave = ActiveCpuCreature; // Save current CPU creature

                // Filter to only those team members that are not weak to opponent
                Creature candidate = cpuActor.Creatures
                    .Where(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type) >= 1.0f)
                    .OrderByDescending(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type))
                    .FirstOrDefault();

                if ((candidate != null) && ActiveCpuCreature != candidate && candidate.alive == true)
                {
                    return ActionType.Swap;
                }
            }
            if (ActiveCpuCreature.curse != Elements.none)
            {
                if (cpuPercentage > 35)
                {
                    return randomNumber < 50 ? ActionType.Attack : ActionType.Defend; // 50/50
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
            else if (ActivePlayerCreature.curse != Elements.none)
            {
                return ActionType.Attack;
            }
            else if (ActivePlayerCreature.curse == Elements.none)
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
        public void ControlLogs()
        {
            while (logs.Count > 10)
            {
                logs.RemoveAt(0); // Remove oldest log
            }
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
                        logs.Add(c.name + " (Player Groundcrasher) fainted");
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
                        logs.Add(c.name + " (Cpu Groundcrasher) fainted");
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
            AllCreatures = JsonConvert.DeserializeObject<List<Creature>>(
                text,
                new JsonSerializerSettings
                {
                    Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                }) ?? new List<Creature>();
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
                logs.Add("the playerActor is null");
                return;
            }
            else
            {
                if(playerActor.Creatures.Count < _maxCreatures)
                {
                    foreach (Creature creature in playerActor.Creatures)
                    {
                        if (creature.id == newCreature.id)
                        {
                            logs.Add("you already have that groundcrasher");
                            return;
                        }
                    }
                    playerActor.Creatures.Add(newCreature.Clone());
                    logs.Add(newCreature.name + " added to team");
                }
                else
                {
                    logs.Add($"you have the maximum GroundCrashers 0f {_maxCreatures}");
                    return;
                }

                if (playerActor.Creatures.Count == _maxCreatures)
                {
                    // enviroment buff for player
                    EnviromentBuff_Setup(playerActor);
                }
            }
        }

        private void EnviromentBuff_Setup(Actor actor)
        {
            if (actor != null)
            {
                foreach (Creature creature in actor.Creatures)
                {
                    // weather buff
                    string actionWeather = WeatherChart.GetWeatherEffectiveness(creature);
                    string[] actionWeatherSplit = actionWeather.Split('/');
                    environmentBuff(creature, actionWeatherSplit);

                    // time of day buff
                    string actionTime = DaytimeChart.GetDaytimeEffectiveness(creature);
                    string[] actionTimeSplit = actionTime.Split('/');
                    environmentBuff(creature, actionTimeSplit);

                    // Biome buff
                    string actionBiome = BiomeChart.GetBiomeEffectiveness(creature);
                    string[] actionBiomeSplit = actionBiome.Split('/');
                    environmentBuff(creature, actionBiomeSplit);
                }
            }
        }

        private void environmentBuff(Creature creature, string[] actionSplit)
        {
            float number = 1.0f;
            if (actionSplit[0] == "Buff") number = _rnd.Next(105, 111) / 100f;
            else if (actionSplit[0] == "Debuff") number = _rnd.Next(90, 96) / 100f;

            switch (actionSplit[1])
            {
                case "Health":
                    creature.stats.max_hp = (int)Math.Round(creature.stats.max_hp * number)
                    break;
                case "Attack":
                    creature.stats.max_attack = (int)Math.Round(creature.stats.max_attack * number);
                    creature.stats.attack = creature.stats.max_attack; // Reset current attack to max after buff
                    break;
                case "Defense":
                    creature.stats.max_defense = (int)Math.Round(creature.stats.max_defense * number);
                    creature.stats.defense = creature.stats.max_defense; // Reset current defense to max after buff
                    break;
                case "Speed":
                    creature.stats.max_speed = (int)Math.Round(creature.stats.max_speed * number);
                    creature.stats.speed = creature.stats.max_speed; // Reset current speed to max after buff
                    break;
            }
        }

        public void LoadActorsForBattleMode()
        {
            CurrentActors.Clear();

            // Create CPU Actor
            var cpuActor = new Actor("CPU", false);
            cpuActor.Creatures.AddRange(GetRandomCreatures(_maxCreatures));
            CurrentActors.Add(cpuActor);

            // enviroment buff for CPU
            EnviromentBuff_Setup(cpuActor);

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
