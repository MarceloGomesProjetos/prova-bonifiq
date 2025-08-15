using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    // A classe agora é abstrata, pois ela não será instanciada diretamente
    public abstract class BaseService<T> where T : class
    {
        protected readonly TestDbContext _ctx;
        
        public BaseService(TestDbContext ctx)
        {
            _ctx = ctx;
        }

        // Método genérico para paginar qualquer entidade
        public async Task<PagedList<T>> GetPagedAsync(int page)
        {
            const int pageSize = 10;
            
            if (page < 1)
            {
                page = 1;
            }

            var itemsToSkip = (page - 1) * pageSize;

            // Acessa o DbSet correspondente ao tipo T
            var query = _ctx.Set<T>();
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip(itemsToSkip)
                .Take(pageSize)
                .ToListAsync();

            var hasNext = itemsToSkip + pageSize < totalCount;

            return new PagedList<T>
            {
                TotalCount = totalCount,
                HasNext = hasNext,
                Items = items
            };
        }
    }
}