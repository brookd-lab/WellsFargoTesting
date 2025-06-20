using InventoryManagement;

var mouse = new Product(id: 1, name: "mouse", quantity: 2, price: 10.99m, lowStockThreshold: 5);
var keyboard = new Product(id: 2, name: "keyboard", quantity: 5, price: 15.99m, lowStockThreshold: 10);
var lamp = new Product(id: 3, name: "lamp", quantity: 15, price: 15.99m, lowStockThreshold: 10);
var guitar = new Product(id: 4, name: "guitar", quantity: 3, price: 25.35m, lowStockThreshold: 2);

var inventory = new Inventory();
inventory.AddProduct(mouse);
inventory.AddProduct(keyboard);
inventory.AddProduct(lamp);
inventory.AddProduct(guitar);

var lowStockReportService = new LowStockReportService(inventory);
lowStockReportService.DisplayLowStockReport();

var inventoryFileService = new InventoryFileService(inventory);
var fileName = @"c:\temp1\test.txt";
bool saved = inventoryFileService.SaveInventoryToFile(fileName);
if (!saved)
    Console.WriteLine($"[Error] Writing to file path: {fileName}");

Console.WriteLine($"Loading inventory from file: {fileName}");
var loadedInventory = inventoryFileService.LoadInventoryFromFile(fileName, out bool successfulLoad);
if (successfulLoad && loadedInventory != null)
{
    // Test the loaded inventory
    loadedInventory.DisplayAllProducts();
}
else
{
    // Handle the failure
    Console.WriteLine($"[Error] Could not load inventory from file: {fileName}");
}

//var singleton1 = Singleton.GetSingleton();
//var singleton2 = Singleton.GetSingleton();

//singleton1.DisplayMessage("Test1");
//singleton2.DisplayMessage("Test2");

public class Singleton
{
    private static Singleton instance;
    private Singleton() { }

    public static Singleton GetSingleton()
    {
        if (instance == null)
        {
            instance = new Singleton();
        }
        return instance;
    }

    public void DisplayMessage(string message)
    {
        Console.WriteLine(message);
    }
}


