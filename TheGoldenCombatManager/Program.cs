using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace TheGoldenCombatManager
{
    class Combatant(int id, bool player, string name, string type, int maxHealth, int ac, List<int> speed, List<int> abilityscores, int proficiency, List<Actions> actions)
    {
        public int ID { get; set; } = id;
        public bool Player { get; set; } = player;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public int MaxHealth { get; set; } = maxHealth;
        public int AC { get; set; } = ac;
        public List<int> Speed { get; set; } = speed;
        public List<int> Abilityscores { get; set; } = abilityscores;
        public int Proficiency { get; set; } = proficiency;
        public List<Actions> Actions { get; set; } = actions;

    }
    class Actions(string name, int range, int dietype, int dieamount, int dmgmod, int tohitmod)
    {
        public string Name { get; set; } = name;
        public int Range { get; set; } = range;
        public int Dietype { get; set; } = dietype;
        public int Dieamount { get; set; } = dieamount;
        public int Dmgmod { get; set; } = dmgmod;
        public int Tohitmod { get; set; } = tohitmod;
    }
    class Fighter(Combatant template, int tempID, int initiative, bool dead)
    {
        public int TempID { get; set; } = tempID;
        public Combatant Template { get; set; } = template;
        public int Health { get; set; } = template.MaxHealth;
        public int Initiative { get; set; } = initiative;
        public bool Dead { get; set; } = dead;
    }
    class Encounter(List<Fighter> turnorder, string name, int id)
    {
        public List<Fighter> TurnOrder { get; set; } = turnorder;
        public string Name { get; set; } = name;
        public int ID { get; set; } = id;
    }

    internal class Program
    {
        const string CombatantsFile = "Combatants.json";
        const string EncountersFile = "Encounters.json";

        static List<Combatant> Combatants = [];
        static List<Encounter> Encounters = [];

        static readonly Action SaveCombatants = () => SaveManager.ListToJsonFile(in Combatants, CombatantsFile);
        static readonly Action LoadCombatants = () => SaveManager.JsonFileToList(CombatantsFile, out Combatants);

        static readonly Action SaveEncounters = () => SaveManager.ListToJsonFile(in Encounters, EncountersFile);
        static readonly Action LoadEncounters = () => SaveManager.JsonFileToList(EncountersFile, out Encounters);

        static void Main()
        {
            LoadCombatants();
            LoadEncounters();
            Menu();
        }

        static void Menu()
        {
            while (true)
            {
                int choice = InputManager.AskForInt("\n1 : Add combatant\n2 : View combatants\n3 : Add action to combatant\n4 : start a Combat\n5 : Quit");
                switch (choice)
                {
                    case 1:
                        AddCombatant();
                        break;
                    case 2:
                        ViewCombatants();
                        break;
                    case 3:
                        AddAction();
                        break;
                    case 4:
                        Combatmenu();
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("\nInvalid Choice, Please choose again\n");
                        break;
                }
            }
        }

        static void AddCombatant()
        {
            int id = Combatants.Count + 1;
            List<int> speed = [];
            List<int> abilityscores = [];
            List<Actions> actions = [];

            bool player = InputManager.AskForBool("\nInput 'Yes' if this combatant is a player, press Enter if not");
            string name = InputManager.AskForString("Input name of combatant you would like to add");
            if (player != true)
            {
                string type = InputManager.AskForString("Input Creature Type");
                int health = InputManager.AskForInt("Input the combatant's total health");
                int ac = InputManager.AskForInt("Input the combatant's AC");
                int wspeed = InputManager.AskForInt("Input the combatant's walkspeed");
                speed.Add(wspeed);
                int sspeed = InputManager.AskForInt("Input the combatant's swimspeed");
                speed.Add(sspeed);
                int fspeed = InputManager.AskForInt("Input the combatant's flyspeed");
                speed.Add(fspeed);
                int str = InputManager.AskForInt("Input the combatant's strength score");
                abilityscores.Add(str);
                int dex = InputManager.AskForInt("Input the combatant's dexterity score");
                abilityscores.Add(dex);
                int con = InputManager.AskForInt("Input the combatant's constitution score");
                abilityscores.Add(con);
                int Int = InputManager.AskForInt("Input the combatant's intelligence score");
                abilityscores.Add(Int);
                int wis = InputManager.AskForInt("Input the combatant's wisdom score");
                abilityscores.Add(wis);
                int cha = InputManager.AskForInt("Input the combatant's charisma score");
                abilityscores.Add(cha);
                int proficiency = InputManager.AskForInt("Input the combatant's proficiency bonus");
                Combatant combatant = new(id, player, name, type, health, ac, speed, abilityscores, proficiency, actions);
                Combatants.Add(combatant);
            }
            else
            {
                int wspeed = 30;
                speed.Add(wspeed);
                int sspeed = 15;
                speed.Add(sspeed);
                int fspeed = 0;
                speed.Add(fspeed);
                int str = 10;
                abilityscores.Add(str);
                int dex = 10;
                abilityscores.Add(dex);
                int con = 10;
                abilityscores.Add(con);
                int Int = 10;
                abilityscores.Add(Int);
                int wis = 10;
                abilityscores.Add(wis);
                int cha = 10;
                abilityscores.Add(cha);
                Combatant combatant = new(id, player, name, "Player", 0, 0, speed, abilityscores, 0, actions);
                Combatants.Add(combatant);
            }

            SaveCombatants();
        }

        static void AddAction()
        {
            while (true)
            {
                Console.WriteLine("");
                string name = InputManager.AskForString("Input the name of the combatant you would like to add an action to");
                Combatant? c = Combatants.Find(c => c.Name == name);
                if (c != null)
                {
                    string Name = InputManager.AskForString("\nInput the name of the action you would like to add");
                    int range = InputManager.AskForInt("Input the action's range in ft");
                    int dietype = InputManager.AskForInt("Input the action's die type");
                    int dieamount = InputManager.AskForInt("Input the action's die amount");
                    int dmgmod = 9999;
                    while(dmgmod == 9999)
                    {
                        string actiontype = InputManager.AskForString("which stat does this action use from 'STR' 'DEX' 'CON' 'INT' 'WIS' 'CHA'");
                        dmgmod = Getdmgmod(actiontype,c);
                    }
                    int tohitmod = dmgmod + c.Proficiency;

                    Actions action = new(Name, range, dietype, dieamount, dmgmod, tohitmod);
                    c.Actions.Add(action);
                    break;
                }
                else
                {
                    Console.WriteLine("There isn't a combatant with that name, try again");
                }
            }
            SaveCombatants();
        }

        static void ViewCombatants()
        {
            Console.WriteLine("");
            foreach (Combatant combatant in Combatants)
            {
                Console.WriteLine("{0} : {1}", combatant.ID, combatant.Name);
            }
            Console.WriteLine("");

            int id = InputManager.AskForInt("");
            Combatant? combatant1 = Combatants.Find(combatant1 => combatant1.ID == id);
            if (combatant1 != null)
            {
                Console.WriteLine("");
                Console.WriteLine("============================================================================");
                Console.WriteLine("{0} ({1})    AC: {2}", combatant1.Name, combatant1.Type, combatant1.AC);
                Console.WriteLine("HP: {0}    Speed: {1}Ft, Swim: {2}Ft, Fly: {3}Ft    Proficiency Bonus: +{4}", combatant1.MaxHealth, combatant1.Speed[0], combatant1.Speed[1], combatant1.Speed[2], combatant1.Proficiency);
                Console.WriteLine("STR: {0}   DEX: {1}   CON: {2}   INT: {3}   WIS: {4}   CHA: {5}", combatant1.Abilityscores[0], combatant1.Abilityscores[1], combatant1.Abilityscores[2], combatant1.Abilityscores[3], combatant1.Abilityscores[4], combatant1.Abilityscores[5]);
                Console.WriteLine("----------------------------------------------------------------------------");
                Console.WriteLine("Actions:");
                foreach(Actions action in combatant1.Actions)
                {
                    Console.WriteLine("  {0}:   Range: {1}Ft   Tohit: 1d20+{2}   Damage: {3}d{4}+{5}", action.Name, action.Range, action.Tohitmod, action.Dieamount, action.Dietype, action.Dmgmod);
                }
                Console.WriteLine("============================================================================");
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("There isn't a combatant with that ID");
            }
        }

        static void Combatmenu()
        {
            Console.WriteLine("");
            string choice = InputManager.AskForString("'Load' or New");
            switch (choice)
            {
                case "Load":
                    LoadCombat();
                    break;
                default:
                    CreateCombat();
                    break;
            }
        }
        static void Combat(List<Fighter> turnorder)
        {
            Console.WriteLine("");
            int turnsinround  = turnorder.Count;
            int turncounter = 0;
            int roundcounter = 1;

            void Nextturn()
            {
                turncounter++;
                if (turncounter >= turnorder.Count)
                {
                    roundcounter++;
                    turncounter = 0;
                }
            }
            List<Fighter> targets = [];
            while (turnorder.Count > 0)
            {
                targets.Clear();
                Fighter f = turnorder[turncounter];
                if (f.Dead)
                {
                    Nextturn();
                    continue;
                }
                Console.WriteLine("============================================================================");
                Console.WriteLine("COMBAT - ROUND {0}| {1}'s Turn", roundcounter, f.Template.Name);
                Console.WriteLine("============================================================================");
                if (f.Template.Player == true)
                {
                    bool dead = InputManager.AskForBool("Is this player dead? 'Yes' or No");
                    if (dead)
                    {
                        f.Dead = true;
                        Nextturn();
                        continue;
                    }
                    Console.WriteLine("\n============================================================================");
                    Console.WriteLine("ENEMIES:\n");
                    foreach (Fighter fighter in turnorder.Where(fighter => fighter.Template.Player == false))
                    {
                        if(fighter.Dead) continue;
                        Console.WriteLine("{0}: {1}:  AC: {2}  HP: {3}/{4}", fighter.TempID, fighter.Template.Name, fighter.Template.AC, fighter.Health, fighter.Template.MaxHealth);
                        targets.Add(fighter);
                    }
                    Console.WriteLine("============================================================================");
                    while (true)
                    {
                        if(targets.Count == 0) break;
                        int target = InputManager.AskForInt("Select the ID of the target");
                        Fighter? targ = targets.Find(targ => targ.TempID == target);
                        if (targ != null)
                        {
                            int dmg = InputManager.AskForInt("how much damage has this target taken?");
                            Console.Write("Health: {0} ->", targ.Health);
                            targ.Health -= dmg;
                            if (targ.Health <= 0)
                            {
                                Console.WriteLine(" {0} killed", targ.Health);
                                targ.Dead = true;
                                break;
                            }
                            Console.WriteLine(" {0}", targ.Health);
                            break;
                        }
                        else
                        {
                            Console.WriteLine("There isn't a target with that ID, try again");
                        }
                    }

                }
                else
                {
                    Console.WriteLine("\n============================================================================");
                    Console.WriteLine("HP: {0}/{1}    AC: {2}", f.Health, f.Template.MaxHealth, f.Template.AC);
                    Console.WriteLine("Speed: {0}Ft, Swim: {1}Ft, Fly: {2}Ft",f.Template.Speed[0], f.Template.Speed[1], f.Template.Speed[2]);
                    Console.WriteLine("STR: {0}   DEX: {1}   CON: {2}   INT: {3}   WIS: {4}   CHA: {5}", f.Template.Abilityscores[0], f.Template.Abilityscores[1], f.Template.Abilityscores[2], f.Template.Abilityscores[3], f.Template.Abilityscores[4], f.Template.Abilityscores[5]);
                    Console.WriteLine("----------------------------------------------------------------------------");
                    Console.WriteLine("Actions:");
                    foreach (Actions action in f.Template.Actions)
                    {
                        Console.WriteLine("  {0}:   Range: {1}Ft   Tohit: 1d20+{2}   Damage: {3}d{4}+{5}", action.Name, action.Range, action.Tohitmod, action.Dieamount, action.Dietype, action.Dmgmod);
                    }
                    Console.WriteLine("============================================================================\n");
                    while (true)
                    {
                        string attack = InputManager.AskForString("Select the name of the action you would like to use");
                        Actions? Action = f.Template.Actions.Find(Action => Action.Name == attack);
                        if (Action != null)
                        {
                            Console.WriteLine("{0} Used {1}\n", f.Template.Name, attack);
                            Roll(Action);
                            break;
                        }
                    }
                    Console.WriteLine("============================================================================\n");
                }
                Console.ReadKey();
                Nextturn();
            }
        }

        static void LoadCombat()
        {
            if (Encounters.Count == 0)
            {
                Console.WriteLine("");
                Console.WriteLine("There are no saved Encounters.");
                return;
            }

            List<Fighter> turnorder = [];
            while (true)
            {
                Console.WriteLine("");
                foreach (Encounter e in Encounters)
                {
                    Console.WriteLine("{0} : {1}", e.ID, e.Name);
                }
                Console.WriteLine("");
                int id = InputManager.AskForInt("Input the ID of the encounter you would like to Run");
                Encounter? encounter = Encounters.Find(encounter => encounter.ID == id);
                if (encounter != null)
                {
                    turnorder.AddRange(encounter.TurnOrder);
                    foreach (Fighter fighter in turnorder)
                    {
                        fighter.Initiative = GetInitiative(fighter.Template.Name);
                    }
                    SortTurnOrder(ref turnorder);
                    break;
                }
                else
                {
                    Console.WriteLine("There isn't an Encounter with that ID, try again");
                }
            }
            Combat(turnorder);
        }

        static void CreateCombat()
        {
            List<Fighter> turnorder = [];

            int id = Encounters.Count + 1;
            while (true)
            {
                AddToTurnOrder(ref turnorder);
                string choice = InputManager.AskForString("Would you like to Add more 'Yes' or No");
                if (choice != "Yes") break;
            }

            Console.WriteLine("");

            foreach (Fighter fighter in turnorder)
            {
                Console.WriteLine("{0}", fighter.Template.Name);
            }
            Console.WriteLine("");
            string name = InputManager.AskForString("Name this Encounter");

            Encounter encounter = new(turnorder, name, id);
            Encounters.Add(encounter);

            SaveEncounters();
        }

        /// <summary>
        /// Takes a [name] and [amount] from the user.<br></br>
        /// If [name] does not appear in `Combatants`, does nothing.<br></br>
        /// Else, Adds [amount] Fighters with name == [name] to the given turn order.<br></br>
        /// Each Fighter is given a unique tempID.
        /// </summary>
        /// <param name="turnOrder">The turn order to add to.</param>
        static void AddToTurnOrder(ref List<Fighter> turnOrder)
        {
            string name = InputManager.AskForString("What is the name of the Combatant you would like to add?");

            // Get first combatant with matching name. If none match, return early.
            Combatant? combatant = Combatants.Find(combatant => combatant.Name == name);
            if (combatant == null) return;

            int amount = InputManager.AskForInt("How many of this Combatant would you like to add?");

            for (int i = 0; i < amount; i++)
            {
                int tempID = turnOrder.Count;
                Fighter fighter = new(combatant, tempID, 0, false);
                turnOrder.Add(fighter);
            }
        }

        static int GetInitiative(string name)
        {
            string Prompt = "What is " + name + "'s Initiative";
            return InputManager.AskForInt(Prompt);
        }

        static void SortTurnOrder(ref List<Fighter> turnorder)
        {
            turnorder.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));
        }

        static int Getmod(float AbilityScore)
        {
            float temp = (AbilityScore - 10) / 2;
            int Mod = (int)Math.Floor(temp);
            return Mod;
        }
        static int Getdmgmod(string Stat, Combatant c)
        {
            return Stat switch
            {
                "STR" => Getmod(c.Abilityscores[0]),
                "DEX" => Getmod(c.Abilityscores[1]),
                "CON" => Getmod(c.Abilityscores[2]),
                "INT" => Getmod(c.Abilityscores[3]),
                "WIS" => Getmod(c.Abilityscores[4]),
                "CHA" => Getmod(c.Abilityscores[5]),
                _ => 9999,
            };
        }
        static void Roll(Actions action)
        {
            Random rnd = new();
            int tohit = rnd.Next(1,21);
            Console.WriteLine("Attack roll: {0} + {1} = {2}\n", tohit, action.Tohitmod, tohit + action.Tohitmod);
            Console.Write("Damage roll: ");
            int loop = 0;
            int totaldmg = 0;
            while(loop < action.Dieamount)
            {
                int dmg = rnd.Next(1,action.Dietype + 1);
                totaldmg += dmg;
                Console.Write("{0} + ",dmg);
                loop++;
            }
            Console.WriteLine("{0} = {1}", action.Dmgmod, totaldmg + action.Dmgmod);
        }
    }
}

