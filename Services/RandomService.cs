using Microsoft.EntityFrameworkCore;
using ProvaPub.Models;
using ProvaPub.Repository;
using System.Security.Cryptography;

namespace ProvaPub.Services
{
	public class RandomService
	{
		private readonly TestDbContext _ctx;
        private readonly Random _random;

         // Injeção de dependência do DbContext
        public RandomService(TestDbContext ctx)
        {
            _ctx = ctx;
            // Cria uma única instância de Random para reutilização
            _random = new Random();
        }

        public async Task<int> GetRandom()
		{
            var number = 0;

            while (true)
            {
                // Gera um número aleatório entre 0 e 99
                number = _random.Next(100);
                
                // Verifica se o número já existe no banco de dados
                if (!await _ctx.Numbers.AnyAsync(n => n.Number == number))
                {
                    break; // Sai do loop se o número for único
                }
            }    

            // Adiciona o novo número ao contexto do banco de dados
            _ctx.Numbers.Add(new RandomNumber() { Number = number });

            // Salva as mudanças de forma assíncrona
            await _ctx.SaveChangesAsync();

            // Retorna o número gerado
            return number;
		}

	}
}
