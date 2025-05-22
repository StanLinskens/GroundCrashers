using Newtonsoft.Json;
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
            // 1) get Enemy action
            bool cpuDied = false;
            string cpuAction = CpuAction();
            //MessageBox.Show($"CPU Action: {cpuAction}");

            string cpuCurse = CurseEffect(ActiveCpuCreature);
            string playerCurse = CurseEffect(ActivePlayerCreature);

            // 3) Resolve the chosen action
            switch (action)
            {
                case ActionType.Attack:
                    {
                        cpuDied = ActionType_Attack(cpuDied, cpuAction, cpuCurse, playerCurse);
                        break;
                    }

                case ActionType.ElementAttack:
                    {
                        cpuDied = ActionType_ElementAttack(cpuDied, cpuAction, cpuCurse, playerCurse);

                        break;
                    }

                case ActionType.Defend:
                    {
                        cpuDied = ActionType_Defend(cpuDied, cpuAction, cpuCurse, playerCurse);
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

            if ((ActivePlayerCreature != null) && ActivePlayerCreature.curse == "none")
            {
                ActivePlayerCreature.stats.attack = ActivePlayerCreature.stats.max_attack;
                ActivePlayerCreature.stats.defense = ActivePlayerCreature.stats.max_defense;
                ActivePlayerCreature.stats.speed = ActivePlayerCreature.stats.max_speed;
            }

            if (ActiveCpuCreature.curse == "none")
            {
                ActiveCpuCreature.stats.attack = ActiveCpuCreature.stats.max_attack;
                ActiveCpuCreature.stats.defense = ActiveCpuCreature.stats.max_defense;
                ActiveCpuCreature.stats.speed = ActiveCpuCreature.stats.max_speed;
            }

            if (ActivePlayerCreature.curse == "ALL")
            {
                RandomCurse(ActivePlayerCreature);
            }

            if(ActiveCpuCreature.curse == "ALL")
            {
                RandomCurse(ActiveCpuCreature);
            }

            if (ActiveCpuCreature == null)
            {
                MessageBox.Show("You win!");
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

        private bool ActionType_Defend(bool cpuDied, string cpuAction, string cpuCurse, string playerCurse)
        {
            if (cpuAction == "Attack")
            {

                int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);

                if (playerCurse != "SkipTurn")
                {
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because block
                    ActivePlayerCreature.curse = "none";
                }

                if (cpuCurse == "SelfHit")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                }
                else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                { 
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                }

                cpuDied = DeadCheck();
            }
            else if (cpuAction == "ElementAttack")
            {
                int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental
                if (playerCurse != "SkipTurn")
                {
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because block
                }

                if (cpuCurse == "SelfHit")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                    ActiveCpuCreature.curse = ActiveCpuCreature.element;
                }
                else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                {
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                    ActivePlayerCreature.curse = ActiveCpuCreature.element;
                }

                cpuDied = DeadCheck();

                if (playerCurse != "SkipTurn")
                {
                    ActivePlayerCreature.curse = "none";
                }

            }
            else if (cpuAction == "Block")
            {
                if (playerCurse != "SkipTurn")
                {
                    ActivePlayerCreature.curse = "none";
                }
                if (cpuCurse != "SkipTurn")
                {
                    ActiveCpuCreature.curse = "none";
                }
            }
            else if (cpuAction == "Swap")
            {
                if (playerCurse != "SkipTurn")
                {
                    ActivePlayerCreature.curse = "none";
                }
                if (cpuCurse != "SkipTurn")
                {
                    // fix swap for cpu creature
                }
            }

            return cpuDied;
        }

        private bool ActionType_ElementAttack(bool cpuDied, string cpuAction, string cpuCurse, string playerCurse)
        {
            if (cpuAction == "Attack")
            {
                if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
                {
                    int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);

                    if (cpuCurse == "SelfHit")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                    }
                    else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                        DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                        if (playerCurse == "SelfHit")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                            ActivePlayerCreature.curse = ActivePlayerCreature.element;
                        }
                        else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                            ActiveCpuCreature.curse = ActivePlayerCreature.element;
                        }

                        cpuDied = DeadCheck();
                    }
                }
                else
                {
                    int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                    if (playerCurse == "SelfHit")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                        ActivePlayerCreature.curse = ActivePlayerCreature.element;
                    }
                    else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                        ActiveCpuCreature.curse = ActivePlayerCreature.element;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);

                        if (cpuCurse == "SelfHit")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                        }
                        else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                        }

                        cpuDied = DeadCheck();
                    }
                }
            }
            else if (cpuAction == "ElementAttack")
            {
                if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
                {
                    int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                    if (cpuCurse == "SelfHit")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                        ActiveCpuCreature.curse = ActiveCpuCreature.element;
                    }
                    else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                        ActivePlayerCreature.curse = ActivePlayerCreature.element;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                        DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                        if (playerCurse == "SelfHit")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                            ActivePlayerCreature.curse = ActivePlayerCreature.element;
                        }
                        else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                            ActiveCpuCreature.curse = ActivePlayerCreature.element;
                        }

                        cpuDied = DeadCheck();
                    }
                }
                else
                {
                    int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                    if (playerCurse == "SelfHit")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                        ActivePlayerCreature.curse = ActivePlayerCreature.element;
                    }
                    else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                        ActiveCpuCreature.curse = ActivePlayerCreature.element;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                        DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                        if (cpuCurse == "SelfHit")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                            ActiveCpuCreature.curse = ActivePlayerCreature.element;
                        }
                        else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                            ActivePlayerCreature.curse = ActivePlayerCreature.element;
                        }

                        cpuDied = DeadCheck();
                    }
                }
            }
            else if (cpuAction == "Block")
            {
                int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                if (cpuCurse != "SkipTurn")
                {
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because block
                }

                if (playerCurse == "SelfHit")
                {
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                    ActivePlayerCreature.curse = ActivePlayerCreature.element;
                }
                else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                    ActiveCpuCreature.curse = ActivePlayerCreature.element;
                }

                if (cpuCurse != "SkipTurn")
                {
                    ActiveCpuCreature.curse = "none";
                }


                cpuDied = DeadCheck();

            }
            else if (cpuAction == "Swap")
            {
                if (cpuCurse != "SkipTurn")
                {
                    // fix swap for cpu creature
                }

                int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                if (playerCurse == "SelfHit")
                {
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                    ActivePlayerCreature.curse = ActivePlayerCreature.element;
                }
                else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                    ActiveCpuCreature.curse = ActivePlayerCreature.element;
                }
            }

            return cpuDied;
        }

        private bool ActionType_Attack(bool cpuDied, string cpuAction, string cpuCurse, string playerCurse)
        {
            if (cpuAction == "Attack")
            {
                if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
                {
                    int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);

                    if (cpuCurse == "SelfHit")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                    }
                    else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);

                        if (playerCurse == "SelfHit")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                        }
                        else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                        }

                        cpuDied = DeadCheck();
                    }
                }
                else
                {
                    int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);

                    if (playerCurse == "SelfHit")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                    }
                    else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);

                        if (cpuCurse == "SelfHit")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                        }
                        else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                        }

                        cpuDied = DeadCheck();
                    }
                }
            }
            else if (cpuAction == "ElementAttack")
            {
                if (ActiveCpuCreature.stats.speed >= ActivePlayerCreature.stats.speed)
                {
                    int DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                    if (cpuCurse == "SelfHit")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                        ActiveCpuCreature.curse = ActiveCpuCreature.element;
                    }
                    else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                        ActivePlayerCreature.curse = ActiveCpuCreature.element;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);

                        if (playerCurse == "SelfHit")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                        }
                        else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                        }

                        cpuDied = DeadCheck();

                    }
                }
                else
                {
                    int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);

                    if (playerCurse == "SelfHit")
                    {
                        ActivePlayerCreature.stats.hp -= DamageDealt;
                    }
                    else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                    {
                        ActiveCpuCreature.stats.hp -= DamageDealt;
                    }

                    cpuDied = DeadCheck();

                    if (ActivePlayerCreature != null && ActiveCpuCreature != null)
                    {
                        DamageDealt = Damage(ActiveCpuCreature.stats.attack, ActivePlayerCreature.stats.defense);
                        DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because elemental

                        if (cpuCurse == "SelfHit")
                        {
                            ActiveCpuCreature.stats.hp -= DamageDealt;
                            ActiveCpuCreature.curse = ActiveCpuCreature.element;
                        }
                        else if (cpuCurse != "missed" || cpuCurse != "SkipTurn")
                        {
                            ActivePlayerCreature.stats.hp -= DamageDealt;
                            ActivePlayerCreature.curse = ActiveCpuCreature.element;
                        }

                        cpuDied = DeadCheck();
                    }
                }
            }
            else if (cpuAction == "Block")
            {
                int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);

                if (cpuCurse != "SkipTurn")
                {
                    DamageDealt = (int)Math.Round(DamageDealt * 0.2f); //less damage because Block
                    ActiveCpuCreature.curse = "none";
                }

                if (playerCurse == "SelfHit")
                {
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                }
                else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                }

                cpuDied = DeadCheck();
            }
            else if (cpuAction == "Swap")
            {
                int DamageDealt = Damage(ActivePlayerCreature.stats.attack, ActiveCpuCreature.stats.defense);
                if (cpuCurse != "SkipTurn")
                {

                }

                if (playerCurse == "SelfHit")
                {
                    ActivePlayerCreature.stats.hp -= DamageDealt;
                }
                else if (playerCurse != "missed" || playerCurse != "SkipTurn")
                {
                    ActiveCpuCreature.stats.hp -= DamageDealt;
                }
            }

            return cpuDied;
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

            if (ActiveCpuCreature.curse != "none")
            {
                if (cpuPercentage > 35)
                {
                    return "Block";
                }
                else if (playerPercentage > 35)
                {
                    if (randomNumber < 20) return "Block";
                    else if (randomNumber < 55) return "Attack"; // 35% window (20–54)
                    else return "ElementAttack"; // 45%
                }
                else
                {
                    return randomNumber < 25 ? "Block" : "Attack";
                }
            }
            if (ActivePlayerCreature.curse != "none")
            {
                return "Attack";
            }
            if (ActivePlayerCreature.curse == "none")
            {
                if (cpuPercentage > 85)
                {
                    if (playerPercentage > 65)
                    {
                        return randomNumber < 33 ? "Attack" : "ElementAttack"; // 33/66
                    }
                    else
                    {
                        return randomNumber < 15 ? "ElementAttack" : "Attack"; // 15/85
                    }
                }
                if (cpuPercentage > 45)
                {
                    if (playerPercentage > 55)
                    {
                        if (randomNumber < 10) return "Block";
                        else if (randomNumber < 45) return "Attack"; // 35% window (10–44)
                        else return "ElementAttack"; // 55%
                    }
                    else
                    {
                        if (randomNumber < 10) return "Block";
                        else if (randomNumber < 25) return "ElementAttack"; // 15% window (10–24)
                        else return "Attack"; // 75%
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
                        if (randomNumber < 10) return "Block";
                        else if (randomNumber < 35) return "ElementAttack"; // 25% window (10–34)
                        else return "Attack"; // 65%
                    }
                }
            }
            return "Attack"; // Default action
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

            bool cpuDied = false;
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
