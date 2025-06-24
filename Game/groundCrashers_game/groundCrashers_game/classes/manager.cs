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
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;

namespace groundCrashers_game.classes
{
    public class Manager
    {
        public List<Creature> AllCreatures { get; private set; } = new();

        /// <summary>
        /// if _currentActorIndex == 1 then currentActor is player, other actor is 1 - 1 = 0 (cpu actor)
        /// if _currentActorIndex == 0 then currentActor is cpu, other actor is 1 - 0 = 1 (player actor)
        /// </summary>
        public List<Actor> CurrentActors { get; private set; } = new();

        public Creature? ActivePlayerCreature { get; private set; }
        public Creature? ActiveCpuCreature { get; private set; }

        public List<string> logs { get; set; } = new List<string>();

        private static readonly Random _rnd = new Random();

        public bool StoryMode { get; set; } // True = story mode, False = free play

        public string levelName { get; set; } // Default level name

        public bool hardcore { get; set; } = false; // True = hardcore mode, False = normal mode

        public int _maxCreatures { get; set; } = 3;

        public string playerChoise { get; set; } = "none"; // Default player action

        public string cpuChoise { get; set; } = "none"; // Default CPU action

        public bool Win { get; set; } = false; // if won the game, true = won, false = lost

        public bool getFromJson = true; // to diside if we want to get data from local JSON or from web

        // the types of actions available in the game
        public enum ActionType
        {
            Attack,
            ElementAttack,
            Defend,
            Swap
        }

        /// <summary>
        /// Index into CurrentActors: 0 = player, 1 = CPU 
        /// </summary>
        private int _currentActorIndex = 0;

        // Round counter (increments after both actors have taken a turn)
        public int RoundNumber { get; private set; } = 1;

        // get the daytime for the current level
        public Daytimes GetDaytime()
        {
            // if the level exists in the json then return the daytime for that level
            if (Levels.Chart.TryGetValue(levelName, out var level))
            {
                return level.Daytime;
            }
            // if it doesnt exist, then return the default daytime
            else
            {
                Console.WriteLine($"Level {levelName} not found.");
                return Daytimes.Day;
            }
        }

        // get a random daytime from the enum (for not story mode)
        public static Daytimes GetRandomDaytime()
        {
            var values = Enum.GetValues(typeof(Daytimes));
            int index = _rnd.Next(values.Length);
            return (Daytimes)values.GetValue(index);
        }

        // get the biome for the current level
        public Biomes GetBiome()
        {
            // if biome exists in the json, return the biome for that level
            if (Levels.Chart.TryGetValue(levelName, out var level))
            {
                return level.Biome;
            }
            // if not then return the default biome
            else
            {
                Console.WriteLine($"Level '{levelName}' not found in chart.");
                return Biomes.Forest;
            }
        }

        // get a random biome from the enum (for not story mode)
        public static Biomes GetRandomBiome()
        {
            var values = Enum.GetValues(typeof(Biomes));
            int index = _rnd.Next(values.Length);
            return (Biomes)values.GetValue(index);
        }

        // to get a random weather from the enum
        public static Weathers GetRandomWeather()
        {
            var values = Enum.GetValues(typeof(Weathers));
            int index = _rnd.Next(values.Length);
            var selected = (Weathers)values.GetValue(index);
            WeatherChart.currentWeather = selected;
            return selected;
        }

        // load teh game data
        public void LoadGameData()
        {
            try
            {
                // Load creatures
                if (getFromJson) LoadAllCreatures(); // if getFromJson is true, load from local JSON
                else LoadAllCreaturesFromWebAsync(); // if getFromJson is false, load from web


                // if the story mode is enabled, load the levels
                if (StoryMode)
                {
                    // Load levels from local JSON for story mode
                    if (getFromJson) Levels.LoadLevelsFromJson();
                    else Levels.LoadLevelsFromWebAsync(); // if it is false load from web

                    if (Levels.Chart.TryGetValue(levelName, out var level))
                    {
                        logs.Add($"Enemy creatures amount: {level.AmountCreaturesCpu}"); // show how many creatures the enemy has
                        logs.Add($"Player creatures amount: {level.AmountCreaturesPlayer}"); // show how many creatures the player has
                    }
                }
                else
                {
                    logs.Add($"Enemy creatures amount: {_maxCreatures}"); // show how many creatures the enemy has
                    logs.Add($"Player creatures amount: {_maxCreatures}"); // show how many creatures the player has
                }

                // if get from JSON is true, load the game data from local JSON files
                if (getFromJson)
                {
                    // load the game data from local JSON files
                    ElementChart.LoadElementsFromJson();
                    PrimaryChart.LoadPrimariesFromJson();
                    WeatherChart.LoadWeathersFromJson();
                    DaytimeChart.LoadDaytimesFromJson();
                    BiomeChart.LoadBiomesFromJson();
                }
                // if get from JSON is false, load the game data from web
                else
                {
                    // load the game data from web
                    ElementChart.LoadElementsFromWebAsync();
                    PrimaryChart.LoadPrimariesFromWebAsync();
                    WeatherChart.LoadWeathersFromWebAsync();
                    DaytimeChart.LoadDaytimesFromWebAsync();
                    BiomeChart.LoadBiomesFromWebAsync();
                }
            }
            // if there is an error loading the game data, show a message box with the error message
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
            // calculate defense persentage based on defender's stats
            float defense = (100f / 600f) * defender.stats.defense;
            // 50 is max defense percentage
            if (defense > 50) defense = 50;
            // calculate damage based on attacker's stats and defense percentage
            float damage = attacker.stats.attack / 100f * (100f - defense);
            // get the multiplier based on primary type effectiveness
            float multiplier = PrimaryChart.GetPrimaryEffectiveness(attacker.primary_type, defender.primary_type);
            damage *= multiplier;
            logs.Add($"{attacker.name} did {multiplier}x the normal damage");
            // return the damage as an integer, rounded to the nearest whole number
            return (int)Math.Round(damage);
        }

        public int DamageElemental(Creature attacker, Creature defender)
        {
            // calculate defense persentage based on defender's stats
            float defense = (100f / 600f) * defender.stats.defense;
            // 50 is max defense percentage
            if (defense > 50) defense = 50;
            // calculate damage based on attacker's stats and defense percentage
            float damage = attacker.stats.attack / 100f * (100f - defense);
            // get the multiplier based on element effectiveness
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
            // get the player and cpu choices (for display of their choise)
            playerChoise = action.ToString().ToLower();
            cpuChoise = actionCpu.ToString().ToLower();

            // 2) diside who goes first
            if (action == ActionType.Swap)
            {
                // If player chooses to swap, player goes first
                _currentActorIndex = 1;
            }
            else if ((actionCpu == ActionType.Swap) || ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
            {
                // if CPU chooses to swap or CPU is faster, CPU goes first
                _currentActorIndex = 0;
            }
            else
            {
                // if player is faster, player goes first
                _currentActorIndex = 1;
            }

            ActionChoice(action, actionCpu, name);
            // check if any creature died after the action (so it doesnt get to attack anymore)
            cpuDied = DeadCheck(cpuDied, true);

            // now the same happens but for the other actor
            ActionChoice(action, actionCpu, name);
            cpuDied = DeadCheck(cpuDied, false);

            // 3) Check if any creature died after the action
            if (cpuDied == true)
            {
                // get the cpu actor
                Actor cpuActor = GetCpuActor();
                // to check if any cpu creature is still alive and then set the active cpu creature to the first alive creature
                ActiveCpuCreature = cpuActor.Creatures.FirstOrDefault(c => c.alive == true);
            }
        }

        private void ActionChoice(ActionType action, ActionType actionCpu, string name)
        {
            // 3) Identify attacker and defender based on _currentActorIndex

            Actor currentActor = CurrentActors[_currentActorIndex];
            Actor otherActor = CurrentActors[1 - _currentActorIndex];

            // pick the active attacker based on the current actor
            // that is checked by currentActor.IsPlayer if true then attacker is player, else cpu
            Creature? attacker = (currentActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            // pick the active defender based on the other actor
            Creature? defender = (otherActor.IsPlayer)
                ? ActivePlayerCreature
                : ActiveCpuCreature;

            // check if the attacker and defender are not null (check if they are alive)
            if (attacker != null && defender != null)
            {
                // 4) find out curse effect on the one that is attacking
                string curse = CurseEffect(attacker);

                // if it is _currentActorIndex == 0, then it is cpu turn, else it is player turn
                if (_currentActorIndex == 0)
                {
                    // make the action to actionCpu if it is cpu turn
                    action = actionCpu;

                    // log the cpu action
                    logs.Add("Choice " + attacker.name + " (cpu) " + action.ToString().ToLower());
                }
                // player turn
                else
                {
                    // log the player action
                    logs.Add("Choice " + attacker.name + " (player) " + action.ToString().ToLower());
                }

                // 5) Perform the action based on the action type
                switch (action)
                {
                    // if the action is attack
                    case ActionType.Attack:
                        {
                            // if the curse is SelfHit, then attacker hits himself
                            if (curse == "SelfHit")
                            {
                                // calculate the damage dealt to himself
                                int DamageDealt = Damage(attacker, attacker);
                                // deal the damage to himself
                                attacker.stats.hp -= DamageDealt;
                                // log the action
                                logs.Add(attacker.name + " hit himself");
                            }
                            // if the curse is not missed or SkipTurn, then deal damage to the defender
                            else if (curse != "missed" && curse != "SkipTurn")
                            {
                                // calculate the damage dealt to the defender
                                int DamageDealt = Damage(attacker, defender);
                                // deal damage
                                defender.stats.hp -= DamageDealt;
                            }
                            // if the curse is missed, then log that the attacker missed
                            else if (curse == "missed")
                            {
                                logs.Add(attacker.name + " missed");
                            }
                            // if the curse is SkipTurn, then log that the attacker skipped his turn
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                                break;
                        }
                    // if the action is ElementAttack
                    case ActionType.ElementAttack:
                        {
                            // if the curse is SelfHit, then attacker hits himself
                            if (curse == "SelfHit")
                            {
                                // calculate the damage dealt to himself
                                int DamageDealt = DamageElemental(attacker, attacker);
                                // deal the damage to himself
                                attacker.stats.hp -= DamageDealt;
                                // log the actions
                                logs.Add(attacker.name + " hit himself");
                                logs.Add(attacker.name + " curse is " + attacker.element.ToString().ToLower());
                            }
                            // if the curse is not missed or SkipTurn, then deal damage to the defender
                            else if (curse != "missed" && curse != "SkipTurn")
                            {
                                // calculate the damage dealt to the defender
                                int DamageDealt = DamageElemental(attacker, defender);
                                // deal damage
                                defender.stats.hp -= DamageDealt;
                                // log the action
                                logs.Add(defender.name + " curse is " + attacker.element.ToString().ToLower());
                            }
                            // if the curse is missed, then log that the attacker missed
                            else if (curse == "missed")
                            {
                                logs.Add(attacker.name + " missed");
                            }
                            // if the curse is SkipTurn, then log that the attacker skipped his turn
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                            break;
                        }
                    // if the action is defend
                    case ActionType.Defend:
                        {
                            // if the curse is not SkipTurn, then attacker Defends
                            if (curse != "SkipTurn")
                            {
                                // calculate the healing amount
                                int healing = (int)Math.Round(attacker.stats.max_hp * 0.20f); // 20% heal
                                // add the healing to the attacker's hp
                                attacker.stats.hp += healing;
                                // cure the curse
                                attacker.curse = Elements.none;
                            }
                            // if the curse is SkipTurn, then log that the attacker skipped his turn
                            else if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                            break;
                        }
                    // if the action is swap
                    case ActionType.Swap:
                        {
                            // if cpu turn and the curse is not SkipTurn
                            if (_currentActorIndex == 0 && curse != "SkipTurn")
                            {
                                // get the cpu actor
                                Actor cpuActor = GetCpuActor();

                                // get the best candidate to swap to (this will be the same one that came out in CpuAction())
                                Creature candidate = cpuActor.Creatures
                                    .Where(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type) >= 1.0f)
                                    .OrderByDescending(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type))
                                    .FirstOrDefault();

                                // make it the active cpu creature
                                ActiveCpuCreature = candidate;
                                break;
                            }

                            // if player turn and the curse is not SkipTurn
                            else if (_currentActorIndex == 1 && curse != "SkipTurn")
                            {
                                // get the player actor
                                Actor playerActor = GetPlayerActor();
                                // if the name is not "default", then swap to the creature with that name
                                if (name != "default")
                                {
                                    // set the active player creature to the creature with that name
                                    CurrentPlayerCreatureSet(name);
                                    // log the swap action
                                    logs.Add("Swapped to " + ActivePlayerCreature.name);
                                }
                                // if the name was default then log the error
                                else
                                {
                                    logs.Add("Invalid swap index");
                                }
                                break;
                            }
                            // if the curse is skipturn, then log that the attacker skipped his turn
                            if (curse == "SkipTurn")
                            {
                                logs.Add(attacker.name + " skipped turn");
                            }
                            break;
                        }
                    // if the action is unknown give error
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), "Unknown action type.");
                }

                // 6) Advance to the next actor’s turn
                _currentActorIndex = 1 - _currentActorIndex;

                // if the attacker healed himself to more than max hp, cap it at max hp
                if (attacker.stats.hp > attacker.stats.max_hp)
                {
                    attacker.stats.hp = attacker.stats.max_hp; // Cap at max hp
                }

                // this is stan his code so I will leave it here idk what it does
                //  If we’ve just returned to actor 0, that means both have acted → increment round
                if (_currentActorIndex == 0)
                {
                    RoundNumber++;
                    // (Optional) Notify UI that a new round has started:
                    // e.g. OnRoundAdvanced?.Invoke(RoundNumber);
                }

                // check if the attacker is still alive after the action and then reset the stats if not cursed
                if ((attacker != null) && attacker.curse == Elements.none)
                {
                    attacker.stats.attack = attacker.stats.max_attack;
                    attacker.stats.defense = attacker.stats.max_defense;
                    attacker.stats.speed = attacker.stats.max_speed;
                }

                // if the defender is still alive after the action, and his curse is all give hime a random curse
                if ((defender != null) && defender.curse == Elements.ALL)
                {
                    RandomCurse(defender);
                }
            }
        }

        private void RandomCurse(Creature ActiveCreature)
        {
            int randomnumber = _rnd.Next(1, 1001); // 1 t/m 1000

            // so basicly a random curse between all of these 5% chance each
            if (randomnumber <= 51) { ActiveCreature.curse = Elements.Nature; }
            else if (randomnumber <= 102) { ActiveCreature.curse = Elements.Ice; }
            else if (randomnumber <= 153) { ActiveCreature.curse = Elements.Toxic; }
            else if (randomnumber <= 204) { ActiveCreature.curse = Elements.Fire; }
            else if (randomnumber <= 255) { ActiveCreature.curse = Elements.Water; }
            else if (randomnumber <= 306) { ActiveCreature.curse = Elements.Draconic; }
            else if (randomnumber <= 357) { ActiveCreature.curse = Elements.Earth; }
            else if (randomnumber <= 408) { ActiveCreature.curse = Elements.Dark; }
            else if (randomnumber <= 459) { ActiveCreature.curse = Elements.Wind; }
            else if (randomnumber <= 510) { ActiveCreature.curse = Elements.Psychic; }
            else if (randomnumber <= 561) { ActiveCreature.curse = Elements.Light; }
            else if (randomnumber <= 612) { ActiveCreature.curse = Elements.Electric; }
            else if (randomnumber <= 663) { ActiveCreature.curse = Elements.Acid; }
            else if (randomnumber <= 714) { ActiveCreature.curse = Elements.Magnetic; }
            else if (randomnumber <= 765) { ActiveCreature.curse = Elements.Demonic; }
            else if (randomnumber <= 816) { ActiveCreature.curse = Elements.Chaos; }
            else if (randomnumber <= 867) { ActiveCreature.curse = Elements.Cosmic; }
            else if (randomnumber <= 918) { ActiveCreature.curse = Elements.Void; }
            else if (randomnumber <= 969) { ActiveCreature.curse = Elements.Astral; }
            else { ActiveCreature.curse = Elements.GOD; } // 3% chance to get GOD curse
            // if the primary type is Titan and the random number is 250 or lower, then set the curse to GOD
            if (ActiveCreature.primary_type == Primaries.Titan && randomnumber <= 250) ActiveCreature.curse = Elements.GOD; 
        }

        private string CurseEffect(Creature activeCreature)
        {
            // caculate a random number to determine the chance curse effect will happen
            int randomNumber = _rnd.Next(100);

            // check if the active creature has a curse
            if (activeCreature.curse != Elements.none)
            {
                // reset stats to max values (normal values)
                activeCreature.stats.attack = activeCreature.stats.max_attack;
                activeCreature.stats.speed = activeCreature.stats.speed;
                // defense isnt done here becasue it is somethimes more reduced per round

                // switch based on the curse
                switch (activeCreature.curse)
                {
                    // if curse is nature, then reduce speed by 40%
                    case Elements.Nature:
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.60f); // 40% speed reduction 
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        break;
                    // if curse is ice, then reduce hp by 8% and attack by 15%
                    case Elements.Ice:
                        {
                            int save = activeCreature.stats.hp;
                            activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.08f); // 8% max hp damage
                            logs.Add(activeCreature.name + " hp went from " + save + " to " + activeCreature.stats.hp);
                            activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.85f); // 15% attack reduction
                            logs.Add(activeCreature.name + " attack went from " + activeCreature.stats.max_attack + " to " + activeCreature.stats.attack);
                            break;
                        }
                    // if curse is toxic/fire, then reduce hp by 15%
                    case Elements.Toxic:
                    case Elements.Fire:
                        {
                            int save = activeCreature.stats.hp;
                            activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.15f); // 15% max hp damage
                            logs.Add(activeCreature.name + " hp went from " + save + " to " + activeCreature.stats.hp);
                            break;
                        }
                    // if curse is water, then reduce attack and speed by 20%
                    case Elements.Water:
                        activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.80f); // 20% attack reduction
                        logs.Add(activeCreature.name + " attack went from " + activeCreature.stats.max_attack + " to " + activeCreature.stats.attack);
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.80f); // 20% speed reduction 
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        break;
                    // if curse is draconic/light/dark, then there is a 30% chance to miss the attack
                    case Elements.Draconic:
                    case Elements.Light:
                    case Elements.Dark:
                        return randomNumber < 30 ? "missed" : "none";
                    // if curese is earth, then reduce defense by 30%
                    case Elements.Earth:
                        // reset the defense to max defense first
                        activeCreature.stats.defense = activeCreature.stats.max_defense;
                        activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * 0.70f); // 30% defense reduction
                        logs.Add(activeCreature.name + " defense went from " + activeCreature.stats.max_defense + " to " + activeCreature.stats.defense);
                        break;
                    // if the curse is GOD, then deal 15% max hp damage and skip the turn 100% of the time
                    case Elements.GOD:
                        {
                            activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.15f); // 15% max hp damage
                            return "SkipTurn";
                        }
                    // if the curse is wind/demonic, then there is a 32% chance to hit self
                    case Elements.Wind:
                    case Elements.Demonic:
                        return randomNumber < 32 ? "SelfHit" : "none";
                    // if the curse is psychic, then there is a 30% chance to skip the turn
                    case Elements.Psychic:
                        return randomNumber < 30 ? "SkipTurn" : "none";
                    // if the curse is electric, then reduce speed by 30% and there is a 20% chance to skip the turn
                    case Elements.Electric:
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.70f); // 30% speed reduction
                        logs.Add(activeCreature.name + " speed went from " + activeCreature.stats.max_speed + " to " + activeCreature.stats.speed);
                        return randomNumber < 20 ? "SkipTurn" : "none";
                    // if the curse is acid, then reduce defense by 13% each turn
                    case Elements.Acid:
                        activeCreature.stats.defense -= (int)Math.Round(activeCreature.stats.defense * 0.13f); // 13% defence reduction each turn
                        logs.Add(activeCreature.name + " defense went from " + activeCreature.stats.max_defense + " to " + activeCreature.stats.defense);
                        break;
                    // if the curse is magnetic, then there is a 33% chance to change speed, attack or defense to a random value between 1 and 200
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
                    // if the curse is cosmic, then there is a 50% chance to reduce speed by 20% or attack by 20%
                    case Elements.Cosmic:
                        if (RoundNumber % 2 == 0)
                        {
                            activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.80f);
                            logs.Add($"{activeCreature.name} is slowed by cosmic gravity to {activeCreature.stats.speed} speed.");
                        }
                        else
                        {
                            activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.80f);
                            logs.Add($"{activeCreature.name}'s attack is disrupted by space waves to {activeCreature.stats.attack}.");
                        }
                        break;
                    // if the curse is chaos, then there is a change to buff or debuff the stats and 10% chance to change values to a random value between 10 and 200
                    case Elements.Chaos:
                        int[] modifiers = { 90, 110 }; // represents 90% or 110%
                        int attackMod = modifiers[_rnd.Next(2)];
                        int defenseMod = modifiers[_rnd.Next(2)];
                        int speedMod = modifiers[_rnd.Next(2)];

                        activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * attackMod / 100f);
                        activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * defenseMod / 100f);
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * speedMod / 100f);
                        logs.Add($"{activeCreature.name} mutates chaotically: Attack={activeCreature.stats.attack}, Defense={activeCreature.stats.defense}, Speed={activeCreature.stats.speed}");

                        if (_rnd.Next(100) < 10)
                        {
                            activeCreature.stats.attack = _rnd.Next(10, 201);
                            activeCreature.stats.defense = _rnd.Next(10, 201);
                            activeCreature.stats.speed = _rnd.Next(10, 201);
                            logs.Add($"{activeCreature.name} suffered a chaotic explosion! Stats randomized.");
                        }
                        break;
                    // if the curse is void, then reduce hp by 5% and attack/defense/speed by 5% each
                    case Elements.Void:
                        int hpBefore = activeCreature.stats.hp;
                        int atkBefore = activeCreature.stats.attack;
                        int defBefore = activeCreature.stats.defense;
                        int spdBefore = activeCreature.stats.speed;

                        activeCreature.stats.hp -= (int)Math.Round(activeCreature.stats.max_hp * 0.05f);
                        activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.95f);
                        activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * 0.95f);
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.95f);

                        logs.Add($"{activeCreature.name} is drained by the Void. HP: {hpBefore} → {activeCreature.stats.hp}, ATK: {atkBefore} → {activeCreature.stats.attack}, DEF: {defBefore} → {activeCreature.stats.defense}, SPD: {spdBefore} → {activeCreature.stats.speed}");
                        break;
                    // astral curse, reduce stats by 12%
                    case Elements.Astral:
                        activeCreature.stats.attack = (int)Math.Round(activeCreature.stats.max_attack * 0.88f);
                        activeCreature.stats.defense = (int)Math.Round(activeCreature.stats.max_defense * 0.88f);
                        activeCreature.stats.speed = (int)Math.Round(activeCreature.stats.max_speed * 0.88f);

                        logs.Add($"{activeCreature.name} feels disconnected from reality. Stats reduced to Attack: {activeCreature.stats.attack}, Defense: {activeCreature.stats.defense}, Speed: {activeCreature.stats.speed}");

                        break;
                }


            }


            return "none";
        }

        private ActionType CpuAction()
        {
            // random number to determine the action
            int randomNumber = _rnd.Next(100);

            // to later find out if the cpu is weak to the player creature and shuld swap
            float multiplier = PrimaryChart.GetPrimaryEffectiveness(ActiveCpuCreature.primary_type, ActivePlayerCreature.primary_type);

            // get cpu stats
            int cpuHp = ActiveCpuCreature.stats.hp; 
            int cpuMaxHp = ActiveCpuCreature.stats.max_hp;
            int cpuPercentage = (int)Math.Round((float)cpuHp / cpuMaxHp * 100);

            // get player stats
            int playerHp = ActivePlayerCreature.stats.hp;
            int playerMaxHp = ActivePlayerCreature.stats.max_hp;
            int playerPercentage = (int)Math.Round((float)playerHp / playerMaxHp * 100);

            // this is just to troll the player and to make their torture longer
            if (ActivePlayerCreature.curse == Elements.GOD) return ActionType.Defend; // If player is cursed with GOD, CPU will always defend
            // if the random number is less than 33 and the cpu is weak to the player creature, then swap
            if ((randomNumber < 33) && multiplier < 1)
            {
                // get cpu actor
                Actor cpuActor = GetCpuActor();
                // get the current effectiveness of the active cpu creature against the active player creature
                float currentEffectiveness = PrimaryChart.GetPrimaryEffectiveness(ActiveCpuCreature.primary_type, ActivePlayerCreature.primary_type);

                // save the current active CPU creature
                Creature activeCpuCreatureSave = ActiveCpuCreature; // Save current CPU creature

                // Filter to only those team members that are not weak to opponent
                Creature candidate = cpuActor.Creatures
                    .Where(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type) >= 1.0f)
                    .OrderByDescending(p => PrimaryChart.GetPrimaryEffectiveness(p.primary_type, ActivePlayerCreature.primary_type))
                    .FirstOrDefault();

                // if the new creature is not null and is not the same as the current active CPU creature and is alive // they can swap. else they will do a diferend action
                if ((candidate != null) && ActiveCpuCreature != candidate && candidate.alive == true)
                {
                    return ActionType.Swap;
                }
            }
            // if the cpu didnt swap and it has a curse
            if (ActiveCpuCreature.curse != Elements.none)
            {
                // if the cpu has more than 35% hp
                if (cpuPercentage > 35)
                {
                    // 50% chance to attack, 50% chance to defend
                    return randomNumber < 50 ? ActionType.Attack : ActionType.Defend; // 50/50
                }
                // if the player has more than 35% hp
                else if (playerPercentage > 35)
                {
                    // 20% chance to defend, 35% chance to attack, 45% chance to elemental attack
                    if (randomNumber < 20) return ActionType.Defend;
                    else if (randomNumber < 55) return ActionType.Attack; // 35% window (20–54)
                    else return ActionType.ElementAttack; // 45%
                }
                // if cpu has less than 35% hp and player has more than 35% hp
                else
                {
                    // 25% chance to defend, 75% chance to attack
                    return randomNumber < 25 ? ActionType.Defend : ActionType.Attack;
                }
            }
            // if cpu has no curse an the player has curse cpu will attack
            else if (ActivePlayerCreature.curse != Elements.none)
            {
                return ActionType.Attack;
            }
            // if cpu has no curse and player has no curse
            else if (ActivePlayerCreature.curse == Elements.none)
            {
                // if cpu has more than 85% hp
                if (cpuPercentage > 85)
                {
                    // if player has more than 65% hp
                    if (playerPercentage > 65)
                    {
                        // 33% chance to attack, 66% chance to elemental attack
                        return randomNumber < 33 ? ActionType.Attack : ActionType.ElementAttack; // 33/66
                    }
                    else
                    {
                        // 15% chance to element attack, 85% chance to attack
                        return randomNumber < 15 ? ActionType.ElementAttack : ActionType.Attack; // 15/85
                    }
                }
                // if cpu has more than 45% hp but less than 85%
                if (cpuPercentage > 45)
                {
                    // if player has more than 55% hp
                    if (playerPercentage > 55)
                    {
                        // 10% chance to defend, 35% chance to attack, 55% chance to elemental attack
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 45) return ActionType.Attack; // 35% window (10–44)
                        else return ActionType.ElementAttack; // 55%
                    }
                    // if player has less than 55% hp
                    else
                    {
                        // 10% chance to defend, 15% chance to elemental attack, 75% chance to attack
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 25) return ActionType.ElementAttack; // 15% window (10–24)
                        else return ActionType.Attack; // 75%
                    }
                }
                // if cpu has less than 45% hp
                else
                {
                    // if player has more than 55% hp
                    if (playerPercentage > 55)
                    {
                        // 25% chance to attack, 75% chance to elemental attack
                        return randomNumber < 25 ? ActionType.Attack : ActionType.ElementAttack;
                    }
                    else
                    {
                        // 10% chance to defend, 25% chance to elemental attack, 65% chance to attack
                        if (randomNumber < 10) return ActionType.Defend;
                        else if (randomNumber < 35) return ActionType.ElementAttack; // 25% window (10–34)
                        else return ActionType.Attack; // 65%
                    }
                }
            }
            // if nothing else matches, return attack as default action (this shouldnt happen but if it does than this)
            return ActionType.Attack; // Default action
        }

        /// <summary>
        /// to controll the logs so they dont get too big (10 lines max)
        /// </summary>
        public void ControlLogs()
        {
            // while it has more than 10 logs, remove the oldest log till it has 10 logs or less
            while (logs.Count > 10)
            {
                logs.RemoveAt(0); // Remove oldest log
            }
        }

        /// <summary>
        /// to check if the player or cpu died
        /// </summary>
        /// <param name="cpuDied">to return if the cpu dead or not</param>
        /// <param name="firstCheck">if it is firstcheck than the display doesnt get shown for dead actor</param>
        /// <returns></returns>
        private bool DeadCheck(bool cpuDied, bool firstCheck)
        {
            // if the active player creature is not null and its hp is 0 or less
            if ((ActivePlayerCreature != null) && ActivePlayerCreature.stats.hp <= 0)
            {
                // get Player Actor
                Actor playerActor = GetPlayerActor();

                // go through all the creatures of the player actor
                foreach (Creature c in playerActor.Creatures)
                {
                    // find the one that matches the name
                    if (c.name == ActivePlayerCreature.name)
                    {
                        // for display purposes, if it is the first check, then set the player choice to "none"
                        if (firstCheck == true) playerChoise = "none";
                        // make it show as dead
                        c.alive = false;
                        // log that the player creature fainted
                        logs.Add(c.name + " (Player Groundcrasher) fainted");
                    }
                }

                // set the active player creature to null
                ActivePlayerCreature = null;
            }

            // if the active cpu creature is not null and its hp is 0 or less
            if ((ActiveCpuCreature != null) && ActiveCpuCreature.stats.hp <= 0)
            {
                // get CPU Actor
                Actor cpuActor = GetCpuActor();

                // go through all the creatures of the cpu actor
                foreach (Creature c in cpuActor.Creatures)
                {
                    // find the one that matches the name
                    if (c.name == ActiveCpuCreature.name)
                    {
                        // for display purposes, if it is the first check, then set the cpu choice to "none"
                        if (firstCheck == true) cpuChoise = "none";
                        // make it show as dead
                        c.alive = false;
                        // log that the cpu creature fainted
                        logs.Add(c.name + " (Cpu Groundcrasher) fainted");
                        // set cpuDied to true
                        cpuDied = true;
                    }
                }
                // set the active cpu creature to null
                ActiveCpuCreature = null;
            }

            return cpuDied;
        }

        /// <summary>
        /// load all creatures from the local JSON file.
        /// </summary>
        /// <exception cref="FileNotFoundException">if the json file is not found/empty</exception>
        public void LoadAllCreatures()
        {
            // get the path to the creatures.json file
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "..", "data", "creatures.json");
            // check if the file exists, if not throw an exception
            if (!File.Exists(path)) throw new FileNotFoundException("Creatures JSON not found.");

            // put it in the all creatures list
            var text = File.ReadAllText(path);
            AllCreatures = JsonConvert.DeserializeObject<List<Creature>>(
                text,
                new JsonSerializerSettings
                {
                    Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                }) ?? new List<Creature>();
        }

        /// <summary>
        /// load all creatures from the web asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task LoadAllCreaturesFromWebAsync()
        {
            // URL to fetch the creatures.json from
            string url = "https://stan.1pc.nl/GroundCrashers/data/creatures.json";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;

                    if (!response.IsSuccessStatusCode)
                        throw new Exception($"Failed to download creatures.json (Status: {response.StatusCode})");

                    var text = await response.Content.ReadAsStringAsync();

                    // Try parsing with a very loose check first:
                    AllCreatures = JsonConvert.DeserializeObject<List<Creature>>(
                        text,
                        new JsonSerializerSettings
                        {
                            Converters = { new Newtonsoft.Json.Converters.StringEnumConverter() }
                        }) ?? new List<Creature>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"UNEXPECTED EXCEPTION: {ex.GetBaseException().Message}");
            }
        }

        /// <summary>
        /// to add a new creature to the player actor's team.
        /// </summary>
        /// <param name="newCreature">the new creature</param>
        public void AddPlayerCreatures(Creature newCreature)
        {
            
            // get Player Actor
            Actor playerActor = GetPlayerActor();
            if(playerActor == null)
            {
                // if playerActor is null, log the error and return
                logs.Add("the playerActor is null");
                return;
            }
            else
            {
                // if the playerActor is not null, check if the amount of creatures is less than the max amount
                if (playerActor.Creatures.Count < _maxCreatures)
                {
                    // check if the new creature is already in the player's team
                    foreach (Creature creature in playerActor.Creatures)
                    {
                        if (creature.id == newCreature.id)
                        {
                            // if it is already in the team, log the error and return
                            logs.Add("you already have that groundcrasher");
                            return;
                        }
                    }
                    // add a clone to the player's team
                    playerActor.Creatures.Add(newCreature.Clone());
                    // log that the new creature is added to the team
                    logs.Add(newCreature.name + " added to team");
                }
                else
                {
                    // log that the player has the maximum amount of creatures
                    logs.Add($"you have the maximum GroundCrashers 0f {_maxCreatures}");
                    return;
                }

                // if the playerActor has the maximum amount of creatures
                if (playerActor.Creatures.Count == _maxCreatures)
                {
                    // enviroment buff for player
                    EnviromentBuff_Setup(playerActor);
                    // level buff for the player actor
                    PlayerLevelBuff(playerActor);
                }
            }
        }

        private void PlayerLevelBuff(Actor actor)
        {
            if(StoryMode)
            {
                foreach (var creature in actor.Creatures)
                {
                    float creatureBuffAmount = 1.0f + (int)ActiveAccount.Active_LVL / 10f; // 10% buff per level

                    creature.stats.max_hp = (int)Math.Round(creature.stats.max_hp * creatureBuffAmount);
                    creature.stats.hp = creature.stats.max_hp; // Reset current hp to max after buff
                    creature.stats.max_attack = (int)Math.Round(creature.stats.max_attack * creatureBuffAmount);
                    creature.stats.attack = creature.stats.max_attack; // Reset current attack to max after buff
                    creature.stats.max_defense = (int)Math.Round(creature.stats.max_defense * creatureBuffAmount);
                    creature.stats.defense = creature.stats.max_defense; // Reset current defense to max after buff
                    creature.stats.max_speed = (int)Math.Round(creature.stats.max_speed * creatureBuffAmount);
                    creature.stats.speed = creature.stats.max_speed; // Reset current speed to max after buff
                }
            }
        }

        /// <summary>
        /// set the buff for the creatures based on the environment (weather, time of day, biome).
        /// </summary>
        /// <param name="actor">the actor (cpu, player)</param>
        private void EnviromentBuff_Setup(Actor actor)
        {
            // if the actor is not null, go through all the creatures and apply the buffs
            if (actor != null)
            {
                foreach (Creature creature in actor.Creatures)
                {
                    // weather buff
                    string actionWeather = WeatherChart.GetWeatherEffectiveness(creature);
                    // split the actionWeather string by '/' to get the buff/debuff type and stat
                    string[] actionWeatherSplit = actionWeather.Split('/');
                    // add the weather buff/debuff to the creature
                    environmentBuff(creature, actionWeatherSplit);

                    // time of day buff
                    string actionTime = DaytimeChart.GetDaytimeEffectiveness(creature);
                    // split the actionTime string by '/' to get the buff/debuff type and stat
                    string[] actionTimeSplit = actionTime.Split('/');
                    // add the time of day buff/debuff to the creature
                    environmentBuff(creature, actionTimeSplit);

                    // Biome buff
                    string actionBiome = BiomeChart.GetBiomeEffectiveness(creature);
                    // split the actionBiome string by '/' to get the buff/debuff type and stat
                    string[] actionBiomeSplit = actionBiome.Split('/');
                    // add the biome buff/debuff to the creature
                    environmentBuff(creature, actionBiomeSplit);
                }
            }
        }

        /// <summary>
        /// add the enviroment buff/debuff to the creature based on the actionSplit array.
        /// </summary>
        /// <param name="creature">the creature</param>
        /// <param name="actionSplit">if it is is a buff/debuff and the stat that gets effected</param>
        private void environmentBuff(Creature creature, string[] actionSplit)
        {
            // default buff number is 1.0f (no change)
            float number = 1.0f;
            // if it is buff than you get a random number between 1.05 and 1.10 (5% to 10% buff)
            if (actionSplit[0] == "Buff") number = _rnd.Next(105, 111) / 100f;
            // if it is debuff than you get a random number between 0.90 and 0.95 (5% to 10% debuff)
            else if (actionSplit[0] == "Debuff") number = _rnd.Next(90, 96) / 100f;

            // the actionSplit[1] is the stat that gets effected
            switch (actionSplit[1])
            {
                // if the actionSplit[1] is health, then buff/debuff the max hp and reset current hp to max
                case "Health":
                    creature.stats.max_hp = (int)Math.Round(creature.stats.max_hp * number);
                    creature.stats.hp = creature.stats.max_hp; // Reset current hp to max after buff
                    break;
                // if the actionSplit[1] is attack, then buff/debuff the max attack and reset current attack to max
                case "Attack":
                    creature.stats.max_attack = (int)Math.Round(creature.stats.max_attack * number);
                    creature.stats.attack = creature.stats.max_attack; // Reset current attack to max after buff
                    break;
                // if the actionSplit[1] is defense, then buff/debuff the max defense and reset current defense to max
                case "Defense":
                    creature.stats.max_defense = (int)Math.Round(creature.stats.max_defense * number);
                    creature.stats.defense = creature.stats.max_defense; // Reset current defense to max after buff
                    break;
                // if the actionSplit[1] is speed, then buff/debuff the max speed and reset current speed to max
                case "Speed":
                    creature.stats.max_speed = (int)Math.Round(creature.stats.max_speed * number);
                    creature.stats.speed = creature.stats.max_speed; // Reset current speed to max after buff
                    break;
            }
        }

        /// <summary>
        /// load the actors for the battle mode.
        /// mainly used to set up the CPU actor with random or level-based creatures.
        /// </summary>
        public void LoadActorsForBattleMode()
        {
            // create a new CPU actor
            var cpuActor = new Actor("CPU", false);

            // if the story mode is not enabled, then get random creatures for the CPU actor
            if (!StoryMode)
            {
                cpuActor.Creatures.AddRange(GetRandomCreatures(_maxCreatures));
                // add them to the current actors list
                CurrentActors.Add(cpuActor);
            }
            else
            {
                // if the story mode is enabled, then get creatures based on the current level
                GetLevelCreatures(cpuActor);
                // add the CPU actor to the current actors list
                CurrentActors.Add(cpuActor);
                // lvl balance the CPU creatures based on the level
                CpuLVLBalance();
            }

            // enviroment buff for CPU
            EnviromentBuff_Setup(cpuActor);

            // create a new Player actor
            var playerActor = new Actor("Player", true);
            // add the player actor to the current actors list
            CurrentActors.Add(playerActor);


            // ←─── After you have given each actor its list of 3 creatures,
            // pick the “first” one to be the active one on‐screen:
            ActiveCpuCreature = cpuActor.Creatures.Count > 0
                ? cpuActor.Creatures[0]
                : null;
        }

        /// <summary>
        /// here happens the balancing of the CPU creatures based on the level.
        /// </summary>
        private void CpuLVLBalance()
        {
            // Balance CPU creatures based on the level
            if (Levels.Chart.TryGetValue(levelName, out var level))
            {
                // multiplier based on the level strength modifier
                double multiplier = level.StrengthModifier / 100.0;

                // if it is hardcore mode the multiplier is increased by 3x
                if (hardcore) multiplier *= 3; // Increase difficulty in hardcore mode

                // go trough all the creatures of the CPU actor
                foreach (var creature in GetCpuActor().Creatures)
                {
                    // give the creature new stats based on the multiplier
                    creature.stats.max_hp = (int)(creature.stats.max_hp * multiplier);
                    creature.stats.max_attack = (int)(creature.stats.max_attack * multiplier);
                    creature.stats.max_defense = (int)(creature.stats.max_defense * multiplier);
                    creature.stats.max_speed = (int)(creature.stats.max_speed * multiplier);

                    // stats reset to max new after balancing
                    creature.stats.hp = creature.stats.max_hp;
                    creature.stats.attack = creature.stats.max_attack;
                    creature.stats.defense = creature.stats.max_defense;
                    creature.stats.speed = creature.stats.max_speed;
                }
            }
        }

        /// <summary>
        /// get the creatures for the CPU actor based on the current level.
        /// </summary>
        /// <param name="cpuActor">the cpu actor</param>
        private void GetLevelCreatures(Actor cpuActor)
        {
            // In story mode, load creatures from the current level
            if (Levels.Chart.TryGetValue(levelName, out var level))
            {
                // go trough all the creature IDs in the level
                foreach (int creatureId in level.CreatureID)
                {
                    // find it in the array and put it in the veriable
                    var creature = AllCreatures.Find(c => c.id == creatureId);
                    // if it isnt null, then add it to the cpu actor's creatures list
                    if (creature != null)
                    {
                        // add is as a clone to avoid reference issues
                        cpuActor.Creatures.Add(creature.Clone());
                    }
                    // if it is 0, then get a random creature
                    else if (creatureId == 0)
                    {
                        cpuActor.Creatures.AddRange(GetRandomCreatures(1));
                    }
                    // log the error if the creature is not found
                    else
                    {
                        Console.WriteLine($"Creature with ID {creatureId} not found.");
                    }
                }
            }
            // log it if the level is not found in the chart
            else
            {
                Console.WriteLine($"Level '{levelName}' not found in chart.");
            }
        }

        /// <summary>
        /// get random creatures for the CPU actor based on the count provided.
        /// </summary>
        /// <param name="count">the count of allowed creatures</param>
        /// <returns>the random creatures get returned</returns>
        private List<Creature> GetRandomCreatures(int count)
        {
            // new list for the creatures
            var randomCreatures = new List<Creature>();
            var available = new List<Creature>(AllCreatures); // Make a copy so we can remove

            // make the amount
            for (int i = 0; i < count && available.Count > 0; i++)
            {
                // if there are no more available creatures, break the loop
                // get a random index from the available creatures
                int index = _rnd.Next(available.Count);
                // clone the creature to avoid reference issues
                var clonedCreature = available[index].Clone();
                // add it to the new list
                randomCreatures.Add(clonedCreature);
                // remove the creature so there are no duplicates
                available.RemoveAt(index); // Avoid duplicates
            }

            return randomCreatures;
        }

        /// <summary>
        /// get a random gamble creature based on the primary type and element.
        /// </summary>
        /// <param name="primary">primary type</param>
        /// <param name="element">element</param>
        /// <returns>a random creature of said type and element</returns>
        public Creature GetRandomGambleCreature(Primaries primary, Elements element)
        {
            // filter the AllCreatures list by primary type and element
            var filtered = AllCreatures.Where(c => c.primary_type == primary).ToList();
            filtered = filtered.Where(c => c.element == element).ToList();
            // if there are no filtered creatures, return null
            if (filtered.Count == 0)
                return null;

            // create a new random instance
            var random = new Random();
            // return a random creature from the filtered list
            return filtered[random.Next(filtered.Count)];
        }

        public void CurrentPlayerCreatureSet(string name)
        {
            // get player actor and set the active player creature by name
            var playerActor = GetPlayerActor();
            // if it isnt null
            if (playerActor != null)
            {
                // set the active player creature to the first creature that matches the name
                ActivePlayerCreature = playerActor.Creatures.FirstOrDefault(c => c.name == name);
            }
        }

        /// <summary>
        /// get the player actor from the current actors list.
        /// </summary>
        /// <returns>returns the player actor</returns>
        public Actor GetPlayerActor() => CurrentActors.Find(a => a.IsPlayer);

        /// <summary>
        /// get the CPU actor from the current actors list.
        /// </summary>
        /// <returns>returns the cpu actor</returns>
        public Actor GetCpuActor() => CurrentActors.Find(a => !a.IsPlayer);
    }
}
