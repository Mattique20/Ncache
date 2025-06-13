using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class NamedTags
    {
        public void run(ICache _cache, string CacheName)
        {
            try
            {
                _cache = CacheManager.GetCache(CacheName);
                Console.WriteLine("Successfully connected to the cache.");
                var product1 = new Product { Id = 211, Name = "Gaming Mouse", Category = "Peripherals" };
                var product2 = new Product { Id = 212, Name = "Mechanical Keyboard", Category = "Peripherals" };
                var product3 = new Product { Id = 213, Name = "Momitor", Category = "Screen" };

                var HighPriority = new NamedTagsDictionary();
                HighPriority.Add("High", 10);
                var MediumPriority = new NamedTagsDictionary();
                MediumPriority.Add("Medium", 10);
                var LowPriority = new NamedTagsDictionary();
                LowPriority.Add("Low", 10);



                // --- Add Product 1 with Tags ---
                var cacheItem1 = new CacheItem(product1);
                cacheItem1.NamedTags = HighPriority;
                _cache.Insert($"product:{product1.Id}", cacheItem1);

                // --- Add Product 2 with Tags ---
                var cacheItem2 = new CacheItem(product2);
                cacheItem2.NamedTags = MediumPriority; // Not on sale
                _cache.Insert($"product:{product2.Id}", cacheItem2);

                // --- Add Product 2 with Tags ---
                var cacheItem3 = new CacheItem(product3);
                cacheItem3.NamedTags = LowPriority; // Not on sale
                _cache.Insert($"product:{product3.Id}", cacheItem3);

                Console.WriteLine("Added 3 products with tags.");
                List<string> productKeys = new List<string>
                    {
                        $"product:{product1.Id}", // "product:101"
                        $"product:{product2.Id}", // "product:102"
                        $"product:{product3.Id}"  // "product:103"
                    };

                var Item = _cache.GetBulk<Product>(productKeys);
                foreach (var item in Item)
                {
                    Console.WriteLine($"  - Key: {item.Key} has following data {item}");

                }
                CacheItem cacheItem = _cache.GetCacheItem("product:201");
                cacheItem.NamedTags.Remove("High");
                Console.WriteLine($"  after removing high priority data");
                var Item2 = _cache.GetBulk<Product>(productKeys);
                foreach (var item in Item2)
                {
                    Console.WriteLine($"  - Key: {item.Key} has following data {item}");

                }

                /* var HighPriorityItem = _cache.SearchService.GetKeysByTag(HighPriority);
                 Console.WriteLine($"\nFound {onSaleItems.Count} item(s) in the {tags[0]} group:");
                 foreach(var pair in onSaleItems)
                 {
                     Console.WriteLine($"  - Key: {pair}");
                 }*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A critical error occurred: {ex.Message}");
            }
            finally
            {
                // Clean up and close connection
                //_cache?.Dispose(); 
            }
        }
    }
}
