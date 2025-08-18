using ProvaPub.Models;
using ProvaPub.Interfaces;

public class PixPaymentStrategy : IPaymentStrategy
{
    private readonly IPixService _pixService;
    
    public PixPaymentStrategy(IPixService pixService)
    {
        _pixService = pixService;
    }
    
    public bool CanProcess(PaymentType paymentType) => paymentType == PaymentType.Pix;
    
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var result = await _pixService.ProcessPixPaymentAsync(request);
            return new PaymentResult
            {
                IsSuccess = result.IsSuccess,
                TransactionId = result.TransactionId,
                ProcessedAmount = request.Amount,
                ErrorMessage = result.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorMessage = $"Erro no processamento PIX: {ex.Message}"
            };
        }
    }

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, int customerId)
    {
        var request = new PaymentRequest
        {
            Amount = amount,
            CustomerId = customerId
        };
        return await ProcessPaymentAsync(request);
    }
}