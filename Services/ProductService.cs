using ProvaPub.Models;
using ProvaPub.Repository;

namespace ProvaPub.Services
{
    public class ProductService : BaseService<Product>
    {
        // O construtor apenas passa o contexto para a classe base
        public ProductService(TestDbContext ctx) : base(ctx) { }

    }
}
