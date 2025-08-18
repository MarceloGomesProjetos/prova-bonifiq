using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using ProvaPub.Interfaces;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace ProvaPub.Services
{
    public class CustomerService : BaseService<Customer>, ICustomerService
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public CustomerService(TestDbContext ctx, IDateTimeProvider dateTimeProvider) : base(ctx)
        {
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        }

        public async Task<bool> CanPurchase(int customerId, decimal purchaseValue)
        {
            if (customerId <= 0)
                throw new ArgumentOutOfRangeException(nameof(customerId), "O ID do cliente deve ser maior que zero.");

            if (purchaseValue <= 0)
                throw new ArgumentOutOfRangeException(nameof(purchaseValue), "O valor da compra deve ser maior que zero.");

            // Para evitar múltiplas queries e garantir que os pedidos estejam carregados.
            var customer = await _ctx.Customers
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                throw new InvalidOperationException($"O ID do cliente {customerId} não existe.");

            var currentDate = _dateTimeProvider.UtcNow;

            // Regra: Apenas uma compra por mês
            var oneMonthAgo = currentDate.AddMonths(-1);
            var recentOrders = customer.Orders.Where(o => o.OrderDate >= oneMonthAgo);
            if (recentOrders.Any())
                return false;

            // Regra: Primeira compra limitada a R$100,00
            var hasPreviousOrders = customer.Orders.Any();
            if (!hasPreviousOrders && purchaseValue > 100)
                return false;

            // Regra: Compras apenas em horário comercial e dias úteis
            var hour = currentDate.Hour;
            var day = currentDate.DayOfWeek;
            var isBusinessHour = hour >= 8 && hour <= 18;
            var isWeekday = day != DayOfWeek.Saturday && day != DayOfWeek.Sunday;

            if (!isBusinessHour || !isWeekday)
                return false;

            return true;
        }
    }
}
