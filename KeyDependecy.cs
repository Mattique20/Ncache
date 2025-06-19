using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    public  class KeyDependecy
    {
        public void SingleKeyDependency(ICache _cache)
        {
            var masterProduct = new Product { Id = 201, Name = "Gaming Mouse", Price = 79.99 };
            string masterKey = $"Product:{masterProduct.Id}";
            _cache.Insert(masterKey, masterProduct);



            var Product = new Product { Id = 202, Name = "Mechanical Keyboard", Price = 125.50 };

            CacheDependency dependency = new KeyDependency(masterKey);
            CacheItem cacheItem = new CacheItem(Product);
            cacheItem.Dependency = dependency;
            _cache.Add($"Product:{Product.Id}", cacheItem);


            var productKeysToFetch = new List<string>();
            productKeysToFetch.Add("Product:201");
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

            _cache.Remove(masterKey);
            IDictionary<string, Product> retrievedProducts2 = _cache.GetBulk<Product>(productKeysToFetch);
            Console.WriteLine($"Successfully retrieved {retrievedProducts2.Count} products from the cache after deleting master.");

            if (retrievedProducts2.Count > 0)
            {
                Console.WriteLine("Retrieved Products:");
                foreach (var kvp in retrievedProducts2)
                {
                    Console.WriteLine($"  -> Key: {kvp.Key}, Value: {kvp.Value}");
                }
            }
        }
        public void MultiKeyDependency(ICache _cache)
        {
            var masterProduct = new Product { Id = 201, Name = "Gaming Mouse", Price = 79.99 };
            string masterKey = $"Product:{masterProduct.Id}";
            _cache.Insert(masterKey, masterProduct);

            var masterProduct2 = new Product { Id = 203, Name = "Gaming Mouse", Price = 79.99 };
            string masterKey2 = $"Product:{masterProduct2.Id}";
            _cache.Insert(masterKey2, masterProduct2);



            var Product = new Product { Id = 202, Name = "Mechanical Keyboard", Price = 125.50 };


            string[] dependencyKeys = { masterKey, masterKey2};
            CacheDependency dependency = new KeyDependency(dependencyKeys);
            CacheItem cacheItem = new CacheItem(Product);
            cacheItem.Dependency = dependency;
            _cache.Add($"Product:{Product.Id}", cacheItem);


            var productKeysToFetch = new List<string>();
            productKeysToFetch.Add("Product:201");
            productKeysToFetch.Add("Product:203");
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

            _cache.Remove(masterKey);
            IDictionary<string, Product> retrievedProducts2 = _cache.GetBulk<Product>(productKeysToFetch);
            Console.WriteLine($"Successfully retrieved {retrievedProducts2.Count} products from the cache after deleting master.");

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
