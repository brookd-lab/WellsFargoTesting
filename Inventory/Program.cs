using InventoryManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register interface with concrete implementation as singleton
builder.Services.AddSingleton<IInventory, Inventory>();
builder.Services.AddTransient<LowStockReportService>();
builder.Services.AddTransient<InventoryFileService>();

var host = builder.Build();

// Container handles all dependency injection automatically
var lowStockReportService = host.Services.GetRequiredService<LowStockReportService>();
var inventoryFileService = host.Services.GetRequiredService<InventoryFileService>();

// Get the singleton _inventory to add products
var inventory = host.Services.GetRequiredService<IInventory>();

var mouse = new Product(id: 1, name: "mouse", quantity: 2, price: 10.99m, lowStockThreshold: 5);
var keyboard = new Product(id: 2, name: "keyboard", quantity: 5, price: 15.99m, lowStockThreshold: 10);
var lamp = new Product(id: 3, name: "lamp", quantity: 15, price: 15.99m, lowStockThreshold: 10);
var guitar = new Product(id: 4, name: "guitar", quantity: 3, price: 25.35m, lowStockThreshold: 2);

inventory.AddProduct(mouse);
inventory.AddProduct(keyboard);
inventory.AddProduct(lamp);
inventory.AddProduct(guitar);

lowStockReportService.DisplayLowStockReport();

var fileName = @"c:\temp1\test.txt";
bool saved = inventoryFileService.SaveInventoryToFile(fileName);
if (!saved)
    Console.WriteLine($"[Error] Writing to file path: {fileName}");

Console.WriteLine($"Loading _inventory from file: {fileName}");
var loaded = inventoryFileService.LoadInventoryFromFile(fileName);
if (loaded && inventory != null)
{
    // Test the loaded _inventory
    inventory.DisplayAllProducts();
}
else
{
    // Handle the failure
    Console.WriteLine($"[Error] Could not load _inventory from file: {fileName}");
}

//var singleton1 = Singleton.GetSingleton();
//var singleton2 = Singleton.GetSingleton();

//singleton1.DisplayMessage("Test1");
//singleton2.DisplayMessage("Test2");

//public class Singleton
//{
//    private static Singleton instance;
//    private Singleton() { }

//    public static Singleton GetSingleton()
//    {
//        if (instance == null)
//        {
//            instance = new Singleton();
//        }
//        return instance;
//    }

//    public void DisplayMessage(string message)
//    {
//        Console.WriteLine(message);
//    }
//}


