using GreetingApi.Services.Cache;
using GreetingApi.Services.Payment;

namespace GreetingApi.Services.PaymentServices
{
    public class BankTransferPaymentService : IPaymentService
    {
        private readonly ICacheService _cacheService;
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public BankTransferPaymentService(ICacheService cacheService)
        {
            _instanceId = ++_instanceCounter;
            _cacheService = cacheService;
            Console.WriteLine($"BankTransferPaymentService instance #{_instanceId} created at {DateTime.Now:HH:mm:ss.fff}");
        }

        public string ProcessPayment(decimal amount, string currency)
        {
            var transactionId = Guid.NewGuid().ToString("N")[..8];
            _cacheService.Set($"transaction_{transactionId}", $"BT-{amount}-{currency}", TimeSpan.FromMinutes(5));
            return $"Bank Transfer payment of {amount} {currency} processed. Transaction: {transactionId} (Instance #{_instanceId})";
        }

        public string GetProviderName() => "Bank Transfer Processor";
    }

}
