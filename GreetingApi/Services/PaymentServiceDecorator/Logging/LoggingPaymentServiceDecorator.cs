using GreetingApi.Services.Payment;

namespace GreetingApi.Services.PaymentServiceDecorator.Logging
{
    public class LoggingPaymentServiceDecorator : PaymentServiceDecoratorBase
    {
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public LoggingPaymentServiceDecorator(IPaymentService inner) : base(inner)
        {
            _instanceId = ++_instanceCounter;
            Console.WriteLine($"LoggingDecorator #{_instanceId} wrapping {inner.GetProviderName()} at {DateTime.Now:HH:mm:ss.fff}");
        }

        public override string ProcessPayment(decimal amount, string currency)
        {
            Console.WriteLine($"[LOG #{_instanceId}] BEFORE: Processing payment of {amount} {currency} using {_inner.GetProviderName()}");

            var startTime = DateTime.UtcNow;
            try
            {
                var result = base.ProcessPayment(amount, currency);
                var duration = DateTime.UtcNow - startTime;

                Console.WriteLine($"[LOG #{_instanceId}] SUCCESS: Payment completed in {duration.TotalMilliseconds}ms");
                return result;
            }
            catch (Exception ex)
            {
                var duration = DateTime.UtcNow - startTime;
                Console.WriteLine($"[LOG #{_instanceId}] ERROR: Payment failed after {duration.TotalMilliseconds}ms - {ex.Message}");
                throw;
            }
        }

        public override string GetProviderName()
        {
            return $"Logged({_inner.GetProviderName()})";
        }
    }

}
