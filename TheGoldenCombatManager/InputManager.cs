namespace TheGoldenCombatManager
{
    /// <summary>A static class containing useful console input operations.</summary>
    internal static class InputManager
    {
        /// <summary>Prompts the user for a string response.</summary>
        /// <param name="prompt">The prompt displayed to the user.</param>
        /// <returns>The user's response.</returns>
        public static string AskForString(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine()!;
        }

        /// <summary>Prompts the user for a string response.</summary>
        /// <param name="prompt">The prompt displayed to the user.</param>
        /// <returns>The user's response.</returns>
        public static bool AskForBool(string prompt)
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine()!;
            if(input == "Yes")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>Prompts the user for an integer response until a valid response is received.</summary>
        /// <param name="prompt">The prompt displayed to the user.</param>
        /// <returns>The user's response.</returns>
        public static int AskForInt(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if (int.TryParse(Console.ReadLine()!, out int result)) return result;
                Console.WriteLine("Invalid Input. Please input an Integer.");
            }
        }

        /// <summary>Prompts the user for a float response until a valid response is received.</summary>
        /// <param name="prompt">The prompt displayed to the user.</param>
        /// <returns>The user's response.</returns>
        public static float AskForFloat(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if (float.TryParse(Console.ReadLine()!, out float result)) return result;
                Console.WriteLine("Invalid input. Please input a Float.");
            }
        }
    }

}
