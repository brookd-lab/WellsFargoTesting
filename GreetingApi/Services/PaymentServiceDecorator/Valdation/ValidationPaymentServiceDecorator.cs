using GreetingApi.Services.Payment;

namespace GreetingApi.Services.PaymentServiceDecorator.Valdation
{
    public class ValidationPaymentServiceDecorator : PaymentServiceDecoratorBase
    {
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public ValidationPaymentServiceDecorator(IPaymentService inner) : base(inner)
        {
            _instanceId = ++_instanceCounter;
            Console.WriteLine($"ValidationDecorator #{_instanceId} wrapping {inner.GetProviderName()} at {DateTime.Now:HH:mm:ss.fff}");
        }

        public override string ProcessPayment(decimal amount, string currency)
        {
            Console.WriteLine($"[VALIDATION #{_instanceId}] Validating payment: {amount} {currency}");

            // Validation logic
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");

            if (amount > 100000)
                throw new ArgumentException("Amount exceeds maximum limit of 100,000");

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new ArgumentException("Currency must be a valid 3-letter code");

            var supportedCurrencies = new[] { "USD", "EUR", "GBP", "CAD" };
            if (!supportedCurrencies.Contains(currency.ToUpper()))
                throw new ArgumentException($"Currency {currency} is not supported");

            Console.WriteLine($"[VALIDATION #{_instanceId}] Payment validation passed");

            return base.ProcessPayment(amount, currency);
        }

        public override string GetProviderName()
        {
            return $"Validated({_inner.GetProviderName()})";
        }
    }

}
