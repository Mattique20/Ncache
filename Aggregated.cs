using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class Aggregated
    {
        public void Run(ICache _cache)
        {
            string filepath = ".\\product.csv";


            var masterProduct = new Product { Id = 202, Name = "Mechanical Keyboard", Price = 125.50 };
            CacheItem cacheItem = new CacheItem(masterProduct);
            _cache.Insert($"Product:{masterProduct.Id}", cacheItem);


            var Product = new Product { Id = 201, Name = "Mechanical Keyboard", Price = 125.50 };
            CacheItem cacheItem2 = new CacheItem(Product);
            
            _cache.Insert($"Product:{Product.Id}", cacheItem2);
            var aggregateDependency = new AggregateCacheDependency();

            // Adding file and key dependency in aggregate dependency
            aggregateDependency.Dependencies.Add(new FileDependency(filepath));
            aggregateDependency.Dependencies.Add(new KeyDependency($"Product:{masterProduct.Id}"));
            cacheItem2.Dependency = aggregateDependency;

            _cache.Insert($"Product:{Product.Id}", cacheItem);


            
            var productKeysToFetch = new List<string>();
            productKeysToFetch.Add("Product:202");
            productKeysToFetch.Add("Product:201");

            IDictionary<string, Product> retrievedProducts = _cache.GetBulk<Product>(productKeysToFetch);
            Console.WriteLine($"Successfully retrieved {retrievedProducts.Count} products from the cache.");

            if (retrievedProducts.Count > 0)
            {
                Console.WriteLine("Retrieved Products:");
                foreach (var kvp in retrievedProducts)
                {
                    Console.WriteLine($"  -> Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }


            _cache.Remove($"Product:{masterProduct.Id}");

            var productKeysToFetch2 = new List<string>();
            productKeysToFetch.Add("Product:202");
            productKeysToFetch.Add("Product:201");

            IDictionary<string, Product> retrievedProducts2 = _cache.GetBulk<Product>(productKeysToFetch);
            Console.WriteLine($"Successfully retrieved {retrievedProducts2.Count} products from the cache after removing dependency.");

            if (retrievedProducts2.Count > 0)
            {
                Console.WriteLine("Retrieved Products:");
                foreach (var kvp in retrievedProducts2)
                {
                    Console.WriteLine($"  -> Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }
    }
}
