using GreetingApi.Services.Payment;

namespace GreetingApi.Services.PaymentServiceDecorator
{
    public abstract class PaymentServiceDecoratorBase : IPaymentService
    {
        protected readonly IPaymentService _inner;

        protected PaymentServiceDecoratorBase(IPaymentService inner)
        {
            _inner = inner;
        }

        public virtual string ProcessPayment(decimal amount, string currency)
        {
            return _inner.ProcessPayment(amount, currency);
        }

        public virtual string GetProviderName()
        {
            return _inner.GetProviderName();
        }
    }

}
