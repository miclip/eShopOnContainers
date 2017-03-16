using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Microsoft.eShopOnContainers.Services.Basket.API.Model
{
    public class RedisBasketRepository : IBasketRepository
    {
        private ILogger<RedisBasketRepository> _logger;        
        private ConnectionMultiplexer _redis;


        public RedisBasketRepository(ConnectionMultiplexer redis, ILoggerFactory loggerFactory)
        {
            _redis = redis;
           _logger = loggerFactory.CreateLogger<RedisBasketRepository>();

        }

        public async Task<bool> DeleteBasket(string id)
        {
            var database = GetDatabase();
            return await database.KeyDeleteAsync(id.ToString());
        }

        public async Task<CustomerBasket> GetBasket(string customerId)
        {
            var database = GetDatabase();

            var data = await database.StringGetAsync(customerId.ToString());
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public async Task<CustomerBasket> UpdateBasket(CustomerBasket basket)
        {
            var database = GetDatabase();

            var created = await database.StringSetAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));
            if (!created)
            {
                _logger.LogInformation("Problem persisting the item");
                return null;
            }

            _logger.LogInformation("basket item persisted succesfully");
            return await GetBasket(basket.BuyerId);
        }

        private IDatabase GetDatabase()
        {
           return _redis.GetDatabase();
        }
    }
}

