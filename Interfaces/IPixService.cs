using ProvaPub.Models;

namespace ProvaPub.Interfaces
{
    public interface IPixService
    {
        Task<PaymentResult> ProcessPixPaymentAsync(PaymentRequest request);
    }
}