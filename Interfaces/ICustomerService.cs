namespace ProvaPub.Interfaces
{
    public interface ICustomerService
    {
        Task<bool> CanPurchase(int customerId, decimal purchaseValue);
    }
}