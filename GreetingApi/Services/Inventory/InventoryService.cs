using GreetingApi.Services.Cache;
using GreetingApi.Services.Order;

namespace GreetingApi.Services.Inventory
{
    public class InventoryService : IInventoryService
    {
        private readonly IOrderValidator _orderValidator; // Use segregated interface instead
        private readonly ICacheService _cacheService;
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public InventoryService(IOrderValidator orderValidator, ICacheService cacheService)
        {
            _instanceId = ++_instanceCounter;
            _orderValidator = orderValidator;
            _cacheService = cacheService;
            Console.WriteLine($"InventoryService instance #{_instanceId} created at {DateTime.Now:HH:mm:ss.fff}");
        }

        public bool CheckStock(string productId, int quantity)
        {
            Console.WriteLine($"[INVENTORY #{_instanceId}] Checking stock for {quantity}x {productId}");

            var currentStock = _cacheService.Get<int?>($"stock_{productId}") ?? 100;
            return currentStock >= quantity;
        }

        public void ReserveStock(string productId, int quantity)
        {
            Console.WriteLine($"[INVENTORY #{_instanceId}] Reserving {quantity}x {productId}");

            var currentStock = _cacheService.Get<int?>($"stock_{productId}") ?? 100;
            _cacheService.Set($"stock_{productId}", currentStock - quantity, TimeSpan.FromHours(24));
        }

        public void ReleaseStock(string orderId)
        {
            Console.WriteLine($"[INVENTORY #{_instanceId}] Releasing stock for order {orderId}");

            if (!_orderValidator.ValidateOrder(orderId))
            {
                throw new InvalidOperationException($"Order {orderId} not found");
            }

            Console.WriteLine($"[INVENTORY #{_instanceId}] Stock released for order {orderId}");
        }
    }


}
