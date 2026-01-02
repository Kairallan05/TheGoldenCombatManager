using System.Text.Json;

namespace TheGoldenCombatManager
{
    /// <summary>A static class containing useful generic file operations.</summary>
    internal static class SaveManager
    {
        static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        /// <summary>Loads the contents of a JSON file to a List.</summary>
        /// <typeparam name="T">The Item Type of the List.</typeparam>
        /// <param name="fileName">The Name of the File to Load From.</param>
        /// <param name="dest">The Destination List.</param>
        public static void JsonFileToList<T>(string fileName, out List<T> dest)
        {
            try
            {
                string jsonString = File.ReadAllText(fileName);
                dest = JsonSerializer.Deserialize<List<T>>(jsonString, JsonOptions)!;
            } catch
            {
                dest = [];
            }
        }

        /// <summary>Saves the contents of a List to a JSON file.</summary>
        /// <typeparam name="T">The Item Type of the List.</typeparam>
        /// <param name="src">The Source List.</param>
        /// <param name="fileName">The Name of the File to Save To.</param>
        public static void ListToJsonFile<T>(in List<T> src, string fileName)
        {
            string jsonString = JsonSerializer.Serialize(src, JsonOptions);
            File.WriteAllText(fileName, jsonString);
        }
    }
}
