namespace GreetingApi.Services.Order
{
    public interface IOrderValidator
    {
        bool ValidateOrder(string orderId);
    }

}
