using Alachisoft.NCache.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class Groups
    {

        public void run(ICache _cache, string CacheName)
        {
            _cache = CacheManager.GetCache(CacheName);
            Console.WriteLine("Successfully connected to the cache.");

            // The key for the product we want to work with
            string productKey = "product:101";
            string productKey2 = "product:102";
            string productKey3 = "product:103";
            var initialProduct = new Product { Id = 101, Name = "Gaming Laptop", Price = 1499.99 };
            var cacheItem = new CacheItem(initialProduct);
            var initialProduct2 = new Product { Id = 102, Name = "Gaming Laptop", Price = 1499.99 };
            var cacheItem2 = new CacheItem(initialProduct);
            var initialProduct3 = new Product { Id = 103, Name = "Gaming Laptop", Price = 1499.99 };
            var cacheItem3 = new CacheItem(initialProduct);
            cacheItem.Group = "laptop and computers";
            cacheItem2.Group = "laptop and computers";
            cacheItem3.Group = "laptop and computers";
            _cache.Add(productKey, cacheItem);
            _cache.Add(productKey2, cacheItem2);
            _cache.Add(productKey3, cacheItem3);

            string groupName = "laptop and computers";

            ICollection<string> keys = _cache.SearchService.GetGroupKeys(groupName);

            if (keys != null && keys.Count > 0)
            {
                // Iterate over the result
                foreach (var key in keys)
                {
                    Console.WriteLine($"Key '{key}' belongs to '{groupName}' group.");
                }
            }



            IDictionary<string, Product> Datakeys = _cache.SearchService.GetGroupData<Product>(groupName);

            if (Datakeys != null && Datakeys.Count > 0)
            {
                // Iterate over the result
                foreach (var key in Datakeys)
                {
                    Console.WriteLine($"Key '{key.Key}' contain info'{key.Value}' .");
                }
            }

            _cache?.Dispose();

        }
    }
}
