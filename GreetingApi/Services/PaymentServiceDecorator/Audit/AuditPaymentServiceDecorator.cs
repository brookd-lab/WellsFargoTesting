using GreetingApi.Services.Cache;
using GreetingApi.Services.Payment;

namespace GreetingApi.Services.PaymentServiceDecorator.Audit
{
    public class AuditPaymentServiceDecorator : PaymentServiceDecoratorBase
    {
        private readonly ICacheService _cacheService;
        private readonly int _instanceId;
        private static int _instanceCounter = 0;

        public AuditPaymentServiceDecorator(IPaymentService inner, ICacheService cacheService) : base(inner)
        {
            _instanceId = ++_instanceCounter;
            _cacheService = cacheService;
            Console.WriteLine($"AuditDecorator #{_instanceId} wrapping {inner.GetProviderName()} at {DateTime.Now:HH:mm:ss.fff}");
        }

        public override string ProcessPayment(decimal amount, string currency)
        {
            var auditId = Guid.NewGuid().ToString("N")[..8];
            var timestamp = DateTime.UtcNow;

            Console.WriteLine($"[AUDIT #{_instanceId}] Starting audit {auditId} for payment: {amount} {currency}");

            // Pre-audit
            var preAudit = new
            {
                AuditId = auditId,
                Timestamp = timestamp,
                Amount = amount,
                Currency = currency,
                Provider = _inner.GetProviderName(),
                Status = "STARTED"
            };

            _cacheService.Set($"audit_pre_{auditId}", preAudit, TimeSpan.FromHours(24));

            try
            {
                var result = base.ProcessPayment(amount, currency);

                // Post-audit (success)
                var postAudit = new
                {
                    AuditId = auditId,
                    Timestamp = DateTime.UtcNow,
                    Amount = amount,
                    Currency = currency,
                    Provider = _inner.GetProviderName(),
                    Status = "COMPLETED",
                    Result = result,
                    Duration = (DateTime.UtcNow - timestamp).TotalMilliseconds
                };

                _cacheService.Set($"audit_post_{auditId}", postAudit, TimeSpan.FromHours(24));
                Console.WriteLine($"[AUDIT #{_instanceId}] Completed audit {auditId} - SUCCESS");

                return result;
            }
            catch (Exception ex)
            {
                // Post-audit (failure)
                var postAudit = new
                {
                    AuditId = auditId,
                    Timestamp = DateTime.UtcNow,
                    Amount = amount,
                    Currency = currency,
                    Provider = _inner.GetProviderName(),
                    Status = "FAILED",
                    Error = ex.Message,
                    Duration = (DateTime.UtcNow - timestamp).TotalMilliseconds
                };

                _cacheService.Set($"audit_post_{auditId}", postAudit, TimeSpan.FromHours(24));
                Console.WriteLine($"[AUDIT #{_instanceId}] Completed audit {auditId} - FAILED: {ex.Message}");

                throw;
            }
        }

        public override string GetProviderName()
        {
            return $"Audited({_inner.GetProviderName()})";
        }
    }

}
