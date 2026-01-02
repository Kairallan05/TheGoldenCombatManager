using System;
using System.Collections.Generic;
using System.Text;

namespace TheGoldenCombatManager
{
    class Combatant(int id, string name, string type, int maxHealth)
    {
        public int ID { get; set; } = id;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public int MaxHealth { get; set; } = maxHealth;
    }

    class Fighter(Combatant template, int tempID, int initiative)
    {
        public int TempID { get; set; } = tempID;
        public Combatant Template { get; set; } = template;
        public int Health { get; set; } = template.MaxHealth;
        public int Initiative { get; set; } = initiative;
    }

    class Encounter(List<Fighter> turnorder, string name)
    {
        public List<Fighter> TurnOrder { get; set; } = turnorder;
        public string Name { get; set; } = name;
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
                string choice = InputManager.AskForString("\nWould you like to 'Add' a combatant, 'View' Combatants, start a 'Combat' or 'Quit'?");
                switch (choice)
                {
                    case "Add":
                        AddCombatant();
                        break;
                    case "View":
                        ViewCombatants();
                        break;
                    case "Combat":
                        Combat();
                        break;
                    case "Quit":
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
            int id = Combatants.Count;

            string name = InputManager.AskForString("\nInput name of combatant you would like to add");
            string type = InputManager.AskForString("Input Creature Type");
            int health = InputManager.AskForInt("Input Creatures total health");

            Combatant combatant = new(id, name, type, health);
            Combatants.Add(combatant);

            SaveCombatants();
        }

        static void ViewCombatants()
        {
            Console.WriteLine();
            foreach (Combatant combatant in Combatants)
            {
                Console.WriteLine("Name: {0}\n  Type: {1}\n  HP: {2}", combatant.Name, combatant.Type, combatant.MaxHealth);
            }
        }

        static void Combat()
        {
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

        static void LoadCombat()
        {
            if (Encounters.Count == 0)
            {
                Console.WriteLine("There are no saved Encounters.");
                return;
            }

            List<Fighter> turnorder = [];
            while (true)
            {
                int tempid = 0;
                foreach(Encounter e in Encounters)
                {
                    tempid++;
                    Console.WriteLine("{0}:{1}", tempid, e.Name);
                }
                string name = InputManager.AskForString("Input the name of the encounter you would like to load");
                Encounter? encounter = Encounters.Find(encounter => encounter.Name == name);
                if (encounter != null)
                {
                    turnorder.AddRange(encounter.TurnOrder);
                    break;
                }
                else
                {
                    Console.WriteLine("There isn't a Encounter with that name, try again");
                }
            }

            foreach (Fighter fighter in turnorder)
            {
                Console.WriteLine("{0}: {1}", fighter.Template.Name, fighter.Initiative);
            }
        }

        static void CreateCombat()
        {
            List<Fighter> turnorder = [];

            
            while (true)
            {
                AddToTurnOrder(ref turnorder);
                string choice = InputManager.AskForString("Would you like to Add more 'Yes' or No");
                if (choice != "Yes") break;
            }

            SortTurnOrder(ref turnorder);

            Console.WriteLine("");

            foreach (Fighter fighter in turnorder)
            {
                Console.WriteLine("{0}: {1}", fighter.Template.Name, fighter.Initiative);
            }

            string name = InputManager.AskForString("Name this Encounter");

            Encounter encounter = new(turnorder, name);
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
                Fighter fighter = new(combatant, tempID, GetInitiative());
                turnOrder.Add(fighter);
            }
        }

        static int GetInitiative()
        {
            return InputManager.AskForInt("What is this Fighter's Initiative?");
        }

        static void SortTurnOrder(ref List<Fighter> turnorder)
        {
            turnorder.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));
        }
    }
}

