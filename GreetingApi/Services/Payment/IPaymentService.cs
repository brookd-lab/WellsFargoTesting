namespace GreetingApi.Services.Payment
{
    public interface IPaymentService
    {
        string ProcessPayment(decimal amount, string currency);
        string GetProviderName();
    }

}
