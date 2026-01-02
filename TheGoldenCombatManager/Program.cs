using System.Dynamic;
using System.Text.Json;
using System.Xml.Linq;

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
        static readonly List<Combatant> Combatants = [];
        static readonly List<Encounter> encounters = [];
        static List<Combatant> GetCombatants()
        {
            try
            {
                string load = File.ReadAllText("Combatants.json");
                List<Combatant> combatants = JsonSerializer.Deserialize<List<Combatant>>(load)!;
                return combatants;
            }
            catch
            {
                List<Combatant> combatants = [];
                return combatants;
            }
        }
        static List<Encounter> GetEncounters()
        {
            try
            {
                string load = File.ReadAllText("Encounters.json");
                List<Encounter> encounters = JsonSerializer.Deserialize<List<Encounter>>(load)!;
                return encounters;
            }
            catch
            {
                List<Encounter> encounters = [];
                return encounters;
            }

        }

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { WriteIndented = true };
        }

        static void Main()
        {
            Combatants.AddRange(GetCombatants());
            encounters.AddRange(GetEncounters());
            Menu();
        }
        static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\nWould you like to 'Add' a combatant, 'View' Combatants, start a 'Combat' or 'Quit'");
                string? choice = Console.ReadLine()!;
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
            Console.WriteLine("\nInput name of combatant you would like to add");
            string? Name = Console.ReadLine()!;
            Console.WriteLine("Input Creature Type");
            string? Type = Console.ReadLine()!;
            int Health = 0;
            while (Health == 0)
            {
                Console.WriteLine("Input Creatures total health");
                try
                {
                    Health = Convert.ToInt32(Console.ReadLine())!;
                }
                catch
                {
                    Console.WriteLine("Invalid input please only input an integer");
                }
            }
            Combatant combatant = new(id, Name, Type, Health);
            Combatants.Add(combatant);
            var options = JsonOptions();
            string json = JsonSerializer.Serialize(Combatants, options);
            File.WriteAllText("Combatants.json", json);
        }
        static void ViewCombatants()
        {
            Console.WriteLine("");
            foreach (Combatant combatant in Combatants)
            {
                Console.WriteLine("Name: {0}\n  Type: {1}\n  HP: {2}", combatant.Name, combatant.Type, combatant.MaxHealth);
            }
        }
        static void Combat()
        {
            Console.WriteLine("'Load' or New");
            string Choice = Console.ReadLine()!;
            switch (Choice)
            {
                case "Load":
                    Loadcombat();
                    break;
                default:
                    CreateCombat();
                    break;
            }
        }
        static void Loadcombat()
        {
            string name;
            List<Fighter> turnorder = [];
            while (true)
            {
                int tempid = 0;
                foreach(Encounter e in encounters)
                {
                    tempid++;
                    Console.WriteLine("{0}:{1}", tempid, e.Name);
                }
                Console.WriteLine("Input the name of the Encounter you would like to Load?");
                name = Console.ReadLine()!;
                Encounter? encounter = encounters.Find(encounter => encounter.Name == name);
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
            bool loop = true;
            while (loop)
            {
                AddToTurnOrder(ref turnorder);
                Console.WriteLine("Would you like to Add more 'Yes' or No");
                string choice = Console.ReadLine()!;
                if (choice != "Yes")
                {
                    loop = false;
                }
            }
            SortTurnOrder(ref turnorder);
            Console.WriteLine("");
            foreach (Fighter fighter in turnorder)
            {
                Console.WriteLine("{0}: {1}", fighter.Template.Name, fighter.Initiative);
            }
            Console.WriteLine("Name this Encounter");
            string name = Console.ReadLine()!;
            Encounter encounter = new(turnorder,name);
            encounters.Add(encounter);
            var options = JsonOptions();
            string json = JsonSerializer.Serialize(encounters, options);
            File.WriteAllText("Encounters.json", json);
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
            Console.WriteLine("What is the name of the Combatant you would like to add?");

            string name = Console.ReadLine()!;

            // Get first combatant with matching name. If none match, return early.
            Combatant? combatant = Combatants.Find(combatant => combatant.Name == name);
            if (combatant == null) return;

            int amount;
            while (true)
            {
                Console.WriteLine("How many of this Combatant would you like to add?");
                if(int.TryParse(Console.ReadLine()!, out amount)) break;
                Console.WriteLine("Invalid input please only input an integer");
            }

            for (int i = 0; i < amount; i++)
            {
                int tempID = turnOrder.Count;
                Fighter fighter = new(combatant, tempID, GetInitiative());
                turnOrder.Add(fighter);
            }
        }
        static int GetInitiative()
        {
            int initiative;
            while (true)
            {
                Console.WriteLine("What is this Fighter's Initiatave?");
                if (int.TryParse(Console.ReadLine()!, out initiative)) break;
                Console.WriteLine("Invalid input please only input an integer");
            }
            return initiative;
        }
        static void SortTurnOrder(ref List<Fighter> turnorder)
        {
            turnorder.Sort((a, b) => b.Initiative.CompareTo(a.Initiative));
        }
    }
}

