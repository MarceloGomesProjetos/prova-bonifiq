
public class PaymentRequest
{
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public string PaymentData { get; set; } // JSON com dados específicos do pagamento
    public string Currency { get; set; } = "BRL";
    public CreditCardInfo CardInfo { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}
