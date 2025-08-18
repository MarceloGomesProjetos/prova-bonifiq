using ProvaPub.Models;
public interface IPaymentStrategy
{
    Task<PaymentResult> ProcessPaymentAsync(decimal paymentValue, int customerId);
    bool CanProcess(PaymentType paymentType);
}   