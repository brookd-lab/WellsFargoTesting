namespace GreetingApi.Services.Inventory
{
    public interface IInventoryService
    {
        bool CheckStock(string productId, int quantity);
        void ReserveStock(string productId, int quantity);
        void ReleaseStock(string orderId); // This will need OrderService!
    }

}
