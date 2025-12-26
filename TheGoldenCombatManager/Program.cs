using System.Text.Json;

namespace TheGoldenCombatManager
{
    class Combatant(int id, string name, string type)
    {
        public int ID { get; set; } = id;
        public string Name { get; set; } = name;
        public string Type { get; set; } = type;
    }
    internal class Program
    {
        static readonly string load = File.ReadAllText("Combatants.json");
        static readonly List<Combatant> Combatants = JsonSerializer.Deserialize<List<Combatant>>(load)!;
        private static JsonSerializerOptions JsonOptions()
        {
            return new JsonSerializerOptions { WriteIndented = true };
        }

        static void Main()
        {
            Menu();
        }
        static void Menu()
        {
            while (true)
            {
                Console.WriteLine("\nWould you like to 'Add' a combatant or 'View' Combatants");
                string? choice = Console.ReadLine()!;
                switch (choice)
                {
                    case "Add":
                        AddCombatant();
                        break;
                    case "View":
                        ViewCombatants();
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
            Combatant combatant = new(id, Name, Type);
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
                Console.WriteLine("Name: {0}\n  Type: {1}\n",combatant.Name, combatant.Type);
            }
        }
    }
}

