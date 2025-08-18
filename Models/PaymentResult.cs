namespace ProvaPub.Models
{
    public class PaymentResult
    {
        public bool IsSuccess { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public decimal ProcessedAmount { get; set; }

    }
}