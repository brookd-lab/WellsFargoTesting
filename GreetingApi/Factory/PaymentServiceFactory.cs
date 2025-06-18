using GreetingApi.Services.Cache;
using GreetingApi.Services.CreditCardPayment;
using GreetingApi.Services.Payment;
using GreetingApi.Services.PaymentServiceDecorator.Audit;
using GreetingApi.Services.PaymentServiceDecorator.Logging;
using GreetingApi.Services.PaymentServiceDecorator.Valdation;
using GreetingApi.Services.PaymentServices;

namespace GreetingApi.Factory
{
    public class PaymentServiceFactory : IPaymentServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Console.WriteLine($"PaymentServiceFactory created at {DateTime.Now:HH:mm:ss.fff}");
        }

        public IPaymentService CreatePaymentService(PaymentRequest request)
        {
            Console.WriteLine($"Factory creating decorated payment service for: {request.Amount} {request.Currency}, User: {request.UserType}");

            // Step 1: Create the base service
            IPaymentService baseService = request switch
            {
                { Amount: >= 10000 } => _serviceProvider.GetRequiredService<BankTransferPaymentService>(),
                { UserType: "Premium" } => _serviceProvider.GetRequiredService<PayPalPaymentService>(),
                _ => _serviceProvider.GetRequiredService<CreditCardPaymentService>()
            };

            // Step 2: Wrap with decorators (order matters!)
            var cacheService = _serviceProvider.GetRequiredService<ICacheService>();

            // Innermost: Audit (closest to actual service)
            IPaymentService auditedService = new AuditPaymentServiceDecorator(baseService, cacheService);

            // Middle: Validation (validates before audit)
            IPaymentService validatedService = new ValidationPaymentServiceDecorator(auditedService);

            // Outermost: Logging (logs everything including validation)
            IPaymentService loggedService = new LoggingPaymentServiceDecorator(validatedService);

            Console.WriteLine($"Created decorated service chain: {loggedService.GetProviderName()}");
            return loggedService;
        }
    }


}
