using GreetingApi.Services.Cache;
using GreetingApi.Services.Inventory;

namespace GreetingApi.Services.Order
{
    public class OrderService : IOrderService, IOrderValidator
    {
        private readonly IInventoryService _inventoryService;
        private readonly ICacheService _cacheService;
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        // Remove circular dependency - only depends on InventoryService
        public OrderService(IInventoryService inventoryService, ICacheService cacheService)
        {
            _instanceId = ++_instanceCounter;
            _inventoryService = inventoryService;
            _cacheService = cacheService;
            Console.WriteLine($"OrderService instance #{_instanceId} created at {DateTime.Now:HH:mm:ss.fff}");
        }

        public string CreateOrder(string productId, int quantity)
        {
            Console.WriteLine($"[ORDER #{_instanceId}] Creating order for {quantity}x {productId}");

            if (!_inventoryService.CheckStock(productId, quantity))
            {
                throw new InvalidOperationException($"Insufficient stock for product {productId}");
            }

            var orderId = $"ORD-{Guid.NewGuid().ToString("N")[..8]}";
            _inventoryService.ReserveStock(productId, quantity);

            _cacheService.Set($"order_{orderId}", new { ProductId = productId, Quantity = quantity, Status = "Created" }, TimeSpan.FromHours(24));

            Console.WriteLine($"[ORDER #{_instanceId}] Order {orderId} created successfully");
            return orderId;
        }

        public bool ValidateOrder(string orderId)
        {
            var order = _cacheService.Get<object>($"order_{orderId}");
            return order != null;
        }
    }


}
