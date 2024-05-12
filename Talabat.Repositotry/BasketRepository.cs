using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Repositotry
{
    public class BasketRepository : IBasketRepository
    {
        private readonly StackExchange.Redis.IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connection )
        {
            _database = connection.GetDatabase();
        }

        public async Task<CustomerBasket?> GetCustomerAsync(string basketId)
        {
            var basket =  await  _database.StringGetAsync(basketId);
            return basket.IsNullOrEmpty?null:JsonSerializer.Deserialize<CustomerBasket>(basket);
        }

        public  async Task<CustomerBasket?> UpdateCustomerAsync(CustomerBasket basket)
        {
            var updatedOrCreated = await _database.StringSetAsync(basket.Id,  JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
            if (updatedOrCreated is false) return null;
            return await GetCustomerAsync(basket.Id);

        }

        public async Task<bool> DeleteCustomerAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }
    }
}
