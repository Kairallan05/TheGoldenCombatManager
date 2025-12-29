using System.Dynamic;
using System.Text.Json;

namespace TheGoldenCombatManager
{
    class Combatant(int id, string name, string type, int MaxHealth)
    {
        public int ID { get; set; } = id;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
        public int MaxHealth { get; set; } = MaxHealth;
    }
    class Figther(int id, string name, string type, int health, int TempID) : Combatant(id, name, type, health)
    {
        public int TempID { get; set; } = TempID;
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
            List<Figther> Fighters = [];
            bool loop = true;
            while (loop)
            {
                Fighters.AddRange(AddToTurnOrder());
                Console.WriteLine("Would you like to Add more 'Yes' or No");
                string choice = Console.ReadLine()!;
                if (choice != "Yes")
                {
                    loop = false;
                }
            }
            Console.WriteLine("");
            foreach (Figther f in Fighters)
            {
                Console.WriteLine("{0}: {1}", f.Name, f.Health);
            }
        }
        static List<Figther> AddToTurnOrder()
        {
            List<Figther> NewFighters = [];
            Console.WriteLine("what is the name of the Comabatant you would like to add");
            string name = Console.ReadLine()!;
            foreach (Combatant combatant in Combatants)
            {
                if (combatant.Name == name)
                {
                    int amount = 0;
                    while (amount == 0)
                    {
                        Console.WriteLine("How many of this Combatant would you like to add?");
                        try
                        {
                            amount = Convert.ToInt32(Console.ReadLine())!;
                        }
                        catch
                        {
                            Console.WriteLine("Invalid input please only input an integer");
                        }
                    }
                    for (int i = 0; i < amount; i++)
                    {
                        int tempID = NewFighters.Count + 1;
                        Figther f = new(combatant.ID, combatant.Name, combatant.Type, combatant.MaxHealth, tempID);
                        NewFighters.Add(f);
                    }
                }
            }
            return NewFighters;
        }
    }
}

