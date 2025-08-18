using ProvaPub.Models;


public class CreditCardService : ICreditCardService
{
    public async Task<PaymentResult> ProcessAsync(PaymentRequest request)
    {
        // Simulação de processamento assíncrono
        await Task.Delay(500); // Simula chamada externa

        if (!ValidateCard(request.CardInfo))
        {
            return new PaymentResult
            {
                IsSuccess = false,
                ErrorMessage = "Cartão inválido."
            };
        }

        // Aqui integraria com um gateway real (ex: Stripe, Cielo, etc.)
        return new PaymentResult
        {
            IsSuccess = true,
            ErrorMessage = "Pagamento aprovado.",
            TransactionId = Guid.NewGuid().ToString()
        };
    }

    public async Task<PaymentResult> ProcessCreditCardAsync(PaymentRequest request)
    {
        // Delegando para o método existente
        return await ProcessAsync(request);
    }

    public bool ValidateCard(CreditCardInfo cardInfo)
    {
        // Validação simples: número e validade
        if (string.IsNullOrWhiteSpace(cardInfo.Number) || cardInfo.Expiry < DateTime.UtcNow)
            return false;

        // Poderia incluir validação de Luhn, CVV, etc.
        return true;
    }

    public bool ValidateCardDetails(CreditCardInfo cardInfo)
    {
        // Delegando para o método existente
        return ValidateCard(cardInfo);
    }
}
