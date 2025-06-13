using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class TAGS
    {
        public void run(ICache _cache, string CacheName)
        {
            try 
            { 
                _cache = CacheManager.GetCache(CacheName);
                Console.WriteLine("Successfully connected to the cache.");
                var product1 = new Product { Id = 201, Name = "Gaming Mouse", Category = "Peripherals" };
                var product2 = new Product { Id = 202, Name = "Mechanical Keyboard", Category = "Peripherals" };


                Tag[] tags = new Tag[3];
                tags[0] = new Tag(" peripherals ");
                tags[1] = new Tag("gaming");
                tags[2] = new Tag("on-sale");

                Tag[] tags2 = new Tag[2];
                tags2[0] = new Tag(" peripherals ");
                tags2[1] = new Tag("gaming");


                // --- Add Product 1 with Tags ---
                var cacheItem1 = new CacheItem(product1);
                cacheItem1.Tags = tags;
                _cache.Insert($"product:{product1.Id}", cacheItem1);

                // --- Add Product 2 with Tags ---
                var cacheItem2 = new CacheItem(product2);
                cacheItem2.Tags = tags2; // Not on sale
                _cache.Insert($"product:{product2.Id}", cacheItem2);

                Console.WriteLine("Added two products with tags.");
                Tag t1 = tags[0];
                var onSaleItems = _cache.SearchService.GetKeysByTag(t1);
                Console.WriteLine($"\nFound {onSaleItems.Count} item(s) in the {tags[0]} group:");
                foreach (var pair in onSaleItems)
                {
                    Console.WriteLine($"  - Key: {pair}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A critical error occurred: {ex.Message}");
            }
            finally
            {
                // Clean up and close connection
                _cache?.Dispose();
            }
        }
    }
}
