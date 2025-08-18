using ProvaPub.Models;
public interface ICreditCardService
{
    Task<PaymentResult> ProcessCreditCardAsync(PaymentRequest request);
    bool ValidateCardDetails(CreditCardInfo cardDetails);
}