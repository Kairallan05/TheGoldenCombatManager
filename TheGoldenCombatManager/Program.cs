using System.Dynamic;
using System.Text.Json;

namespace TheGoldenCombatManager
{
    class Combatant(int id, string name, string type, int maxHealth)
    {
        public int ID { get; set; } = id;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public int MaxHealth { get; set; } = maxHealth;
    }
    class Fighter(int id, string name, string type, int health, int tempID) : Combatant(id, name, type, health)
    {
        public int TempID { get; set; } = tempID;
        public int Health { get; set; } = health;
    }

    internal class Program
    {
        static readonly List<Combatant> Combatants = [];
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

        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { WriteIndented = true };
        }

        static void Main()
        {
            Combatants.AddRange(GetCombatants());
            Menu();
        }
        static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\nWould you like to 'Add' a combatant, 'View' Combatants or start a 'Combat'");
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
                        CreateCombat();
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
        static void CreateCombat()
        {
            List<Fighter> turnOrder = [];
            bool loop = true;
            while (loop)
            {
                AddToTurnOrder(ref turnOrder);
                Console.WriteLine("Would you like to Add more 'Yes' or No");
                string choice = Console.ReadLine()!;
                if (choice != "Yes")
                {
                    loop = false;
                }
            }
            Console.WriteLine("");
            foreach (Fighter fighter in turnOrder)
            {
                Console.WriteLine("{0}: {1}", fighter.Name, fighter.Health);
            }
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
                if (int.TryParse(Console.ReadLine()!, out amount)) break;
                Console.WriteLine("Invalid input please only input an integer");
            }

            for (int i = 0; i < amount; i++)
            {
                int tempID = turnOrder.Count + 1;
                Fighter fighter = new(combatant.ID, combatant.Name, combatant.Type, combatant.MaxHealth, tempID);
                turnOrder.Add(fighter);
            }
        }
    }
}

