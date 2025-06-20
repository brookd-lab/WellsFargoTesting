using System;
using System.IO;
using System.Text.Json;

namespace InventoryManagement
{
    public class InventoryFileService
    {
        private readonly IInventory _inventory;

        public InventoryFileService(IInventory inventory)
        {
            if (inventory is null)
                throw new ArgumentNullException(nameof(inventory));

            this._inventory = inventory;
        }
        public bool SaveInventoryToFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Filepath must not be empty");

            var products = _inventory.GetAllProducts();

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

        public bool LoadInventoryFromFile(string filePath)
        {
            string json = "";
            List<Product> products = new();
            try
            {
                json = File.ReadAllText(filePath);
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine($"File not found for file path: {filePath} {ex.Message}");
                return false;
            }
            catch(UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Unauthorized access for filePath: {filePath} {ex.Message}");
                return false;
            }

            try
            {
                products = JsonSerializer.Deserialize<List<Product>>(json)!;
                _inventory.Clear();
                foreach(var product in products)
                {
                    _inventory.AddProduct(product);
                }
                return true;
            }
            catch(JsonException ex)
            {
                Console.WriteLine($"Invalid Json Format for {filePath} {ex.Message}");
                return false;
            }
        }
    }
}
