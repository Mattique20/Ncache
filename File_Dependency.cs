using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class File_Dependency
    {
        public void Run(ICache _cache)
        {
            string filepath = ".\\product.csv";
            var Product = new Product { Id = 202, Name = "Mechanical Keyboard", Price = 125.50 };
            CacheItem cacheItem = new CacheItem(Product);
           // _cache.Add($"Product:{Product.Id}", cacheItem);


            cacheItem.Dependency = new FileDependency(filepath, DateTime.Now.AddSeconds(1));
            _cache.Insert($"Product:{Product.Id}", cacheItem);

            Console.WriteLine("Dependency added now change smthng in file");

            var productKeysToFetch = new List<string>();
            productKeysToFetch.Add("Product:202");

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




        }

    }
}
