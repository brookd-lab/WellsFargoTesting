namespace GreetingApi.Services.Order
{
    public interface IOrderService
    {
        string CreateOrder(string productId, int quantity);
        bool ValidateOrder(string orderId);
    }

}
