using ProvaPub.Models;
using ProvaPub.Services;
using ProvaPub.Interfaces;

public class CreditCardPaymentStrategy : IPaymentStrategy
{
    private readonly ICreditCardService _creditCardService;

    public CreditCardPaymentStrategy(ICreditCardService creditCardService)
    {
        _creditCardService = creditCardService;
    }

    public bool CanProcess(PaymentType paymentType) => paymentType == PaymentType.CreditCard;

    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var result = await _creditCardService.ProcessCreditCardAsync(request);
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
                ErrorMessage = $"Erro no processamento do cartão de crédito: {ex.Message}"
            };
        }
    }

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, int customerId)
    {
        var paymentRequest = new PaymentRequest
        {
            Amount = amount,
            CustomerId = customerId,
            PaymentType = PaymentType.CreditCard
        };
        return await ProcessPaymentAsync(paymentRequest);
    }
}