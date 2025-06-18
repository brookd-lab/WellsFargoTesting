using GreetingApi.Services.Cache;
using GreetingApi.Services.Payment;

namespace GreetingApi.Services.CreditCardPayment
{
    public class CreditCardPaymentService : IPaymentService
    {
        private readonly ICacheService _cacheService;
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public CreditCardPaymentService(ICacheService cacheService)
        {
            _instanceId = ++_instanceCounter;
            _cacheService = cacheService;
            Console.WriteLine($"CreditCardPaymentService instance #{_instanceId} created at {DateTime.Now:HH:mm:ss.fff}");
        }

        public string ProcessPayment(decimal amount, string currency)
        {
            // Simulate processing logic
            var transactionId = Guid.NewGuid().ToString("N")[..8];
            _cacheService.Set($"transaction_{transactionId}", $"CC-{amount}-{currency}", TimeSpan.FromMinutes(5));
            return $"Credit Card payment of {amount} {currency} processed. Transaction: {transactionId} (Instance #{_instanceId})";
        }

        public string GetProviderName() => "Credit Card Processor";
    }

}
