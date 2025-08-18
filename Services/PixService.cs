using ProvaPub.Interfaces;
using ProvaPub.Models;

namespace ProvaPub.Services 
{
    public class PixService : IPixService
    {
        public async Task<PaymentResult> ProcessPixPaymentAsync(PaymentRequest request)
        {
            // Simulação de processamento de pagamento via Pix
            await Task.Delay(1000); // Simula um atraso no processamento    
            return new PaymentResult
            {
                IsSuccess = true,
                TransactionId = Guid.NewGuid().ToString(),
                ProcessedAmount = request.Amount
            };
        }   

    }
}