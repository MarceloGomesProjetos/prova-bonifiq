using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using ProvaPub.Models;
using ProvaPub.Services;
using ProvaPub.Interfaces;
using ProvaPub.Repository;
using Microsoft.EntityFrameworkCore.Query;

namespace ProvaPub.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<TestDbContext> _ctx;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _ctx = new Mock<TestDbContext>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            _customerService = new CustomerService(_ctx.Object, _dateTimeProviderMock.Object);
        }

        // Cliente inexistente deve lançar exceção
        [Fact]
        public async Task CanPurchase_InvalidCustomer_ThrowsException()
        {
            var customerId = 999;
            
            // Mock do DbSet para o FindAsync.
            // Para testar FindAsync, é preciso mockar o Find<T> diretamente no DbSet.
            var mockDbSet = new Mock<DbSet<Customer>>();
            mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Customer)null);
            
            _ctx.Setup(c => c.Customers).Returns(mockDbSet.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => _customerService.CanPurchase(customerId, 50));
        }

        // Cliente com compra no último mês não pode comprar
        [Fact]
        public async Task CanPurchase_RecentOrder_ReturnsFalse()
        {
            var customerId = 1;
            var now = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(now);

            // Dados de teste em memória
            var customer = new Customer
            {
                Id = customerId,
                Name = "John Doe",
                Orders = new List<Order>
                {
                    new Order { Id = 1, Date = now.AddDays(-5) } // Compra recente
                }
            };

            // Mock do DbSet e da lógica de consulta
            var customers = new List<Customer> { customer }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Customer>>();
            
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            // Mockando o Include, que é um método de extensão
            mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            // Mockando o FirstOrDefaultAsync
            mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Expression<Func<Customer, bool>> predicate, CancellationToken token) =>
                         customers.FirstOrDefault(predicate.Compile()));

            _ctx.Setup(c => c.Customers).Returns(mockDbSet.Object);
            
            // Ativa o teste
            var result = await _customerService.CanPurchase(customerId, 50);

            // Valida o resultado
            Assert.False(result);
        }

        // Cliente sem compra no último mês pode comprar (saldo suficiente não é testado aqui)
        [Fact]
        public async Task CanPurchase_NoRecentOrder_ReturnsTrue()
        {
            var customerId = 1;
            var now = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(now);
            
            var customer = new Customer
            {
                Id = customerId,
                Name = "Jane Smith",
                Orders = new List<Order>
                {
                    new Order { Id = 1, Date = now.AddMonths(-2) } // Compra antiga
                }
            };
            
            // Dados de teste em memória
            var customers = new List<Customer> { customer }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Customer>>();
            
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            // Mockando o Include
            mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);
            
            // Mockando o FirstOrDefaultAsync
            mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Expression<Func<Customer, bool>> predicate, CancellationToken token) =>
                         customers.FirstOrDefault(predicate.Compile()));
            
            _ctx.Setup(c => c.Customers).Returns(mockDbSet.Object);

            var result = await _customerService.CanPurchase(customerId, 50);
            
            Assert.True(result);
        }

        // Cliente com número de compras maior ou igual a 10 não pode comprar (sem base de dados)
        [Fact]
        public async Task CanPurchase_ExceedsTotalOrders_ReturnsFalse()
        {
            var customerId = 1;
            var now = DateTime.UtcNow;
            _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(now);

            // Cria um cliente com 11 compras antigas para testar a regra de limite
            var orders = Enumerable.Range(1, 11).Select(i => new Order { Id = i, Date = now.AddYears(-1) }).ToList();
            
            var customer = new Customer
            {
                Id = customerId,
                Name = "Jane Smith",
                Orders = orders
            };

            var customers = new List<Customer> { customer }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Customer>>();

            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Provider).Returns(customers.Provider);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.Expression).Returns(customers.Expression);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.ElementType).Returns(customers.ElementType);
            mockDbSet.As<IQueryable<Customer>>().Setup(m => m.GetEnumerator()).Returns(customers.GetEnumerator());

            mockDbSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockDbSet.Object);

            mockDbSet.Setup(m => m.FirstOrDefaultAsync(It.IsAny<Expression<Func<Customer, bool>>>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Expression<Func<Customer, bool>> predicate, CancellationToken token) =>
                         customers.FirstOrDefault(predicate.Compile()));

            _ctx.Setup(c => c.Customers).Returns(mockDbSet.Object);

            var result = await _customerService.CanPurchase(customerId, 50);
            
            Assert.False(result);
        }
    }

    // Este código é necessário para que FirstOrDefaultAsync e outros métodos funcionem corretamente com Moq
    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var result = _inner.Execute<TResult>(expression);
            return result;
        }
    }

    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public TestAsyncEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public T Current => _enumerator.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_enumerator.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}