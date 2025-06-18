using GreetingApi.Services.Payment;

namespace GreetingApi.Factory
{
    public interface IPaymentServiceFactory
    {
        IPaymentService CreatePaymentService(PaymentRequest request);
    }

    public record PaymentRequest(decimal Amount, string Currency, string UserType);

}
