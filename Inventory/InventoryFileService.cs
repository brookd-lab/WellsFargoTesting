using System;
using System.IO;
using System.Text.Json;

namespace InventoryManagement
{
    public class InventoryFileService
    {
        private readonly Inventory inventory;

        public InventoryFileService(Inventory inventory)
        {
            if (inventory is null)
                throw new ArgumentNullException(nameof(inventory));

            this.inventory = inventory;
        }
        public bool SaveInventoryToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Filepath must not be empty");

            var products = inventory.GetAllProducts();

            var jsonText = JsonSerializer.Serialize(products);

            try
            {
                return WriteJsonTextToFile(filePath, jsonText);
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"[Info] {ex.Message}, Creating directory");
                var directory = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(directory!);
                return WriteJsonTextToFile(filePath, jsonText);
            }
        }

        private bool WriteJsonTextToFile(string filePath, string jsonText)
        {
            try
            {
                File.WriteAllText(filePath, jsonText);
                Console.WriteLine($"[Success] Writing to file: {filePath}");
                return true;
            }
            catch (DirectoryNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] Writing to file: {filePath}, {ex.Message}");
                return false;
            }
        }

        public Inventory? LoadInventoryFromFile(string filePath, out bool successfulLoad)
        {
            string text = "";
            List<Product> products = new();
            try
            {
                text = File.ReadAllText(filePath);
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine($"File not found for file path: {filePath} {ex.Message}");
                successfulLoad = false;
                return null;
            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for filePath: {filePath} {ex.Message}");
                successfulLoad = false;
                return null;
            }

            try
            {
                products = JsonSerializer.Deserialize<List<Product>>(text)!;
            }
            catch(JsonException ex)
            {
                Console.WriteLine($"Invalid Json Format for {filePath} {ex.Message}");
                successfulLoad = false;
                return null;
            }

            var inventory = new Inventory(products!);
            successfulLoad = true;
            return inventory;
        }
    }
}
