// Program.cs
using System;
using System.Collections.Generic;
using System.Threading;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;
using NcacheDemo;

namespace NCacheDemoApp
{
    class Program
    {
        private static ICache _cache;
        private const string CacheName = "59PORClusteredCache"; 

        static void Main(string[] args)
        {
            Console.WriteLine("NCache Bulk Operations Demo");

            try
            {
                _cache = CacheManager.GetCache(CacheName);
                if (_cache != null)
                {
                    // 1. Create and register the event handler
                    Events eventHandler = new Events();
                    eventHandler.RegisterCacheNotificationsForAllOperations(_cache);
                   


                    Console.WriteLine("--------------------- Part 1: Bulk Insert using a Loop ------------------------------------");
                    int numberOfProducts = 100;
                    Console.WriteLine($"Preparing to insert {numberOfProducts} products in bulk...");


                    var productItems = new Dictionary<string, CacheItem>();
                    for (int i = 1; i <= numberOfProducts; i++)
                    {
                        var product = new Product
                        {
                            Id = 200 + i,
                            Name = $"Bulk Product #{i}",
                            Price = Math.Round(19.99 + (i * 2.5), 2)
                        };
                        string cacheKey = $"product:{product.Id}";
                        productItems.Add(cacheKey, new CacheItem(product));
                    }

                    _cache.AddBulk(productItems);
                    Console.WriteLine($"Bulk insert operation completed for {productItems.Count} items.");
                    Console.WriteLine(new string('-', 40));

                    Thread.Sleep(10000);

                    Console.WriteLine("-------------------------------- Part 2: Bulk Get using a Loop ---------------------------");

                    Console.WriteLine("Preparing to retrieve all products using GetBulk...");
                    var productKeysToFetch = new List<string>();
                    for (int i = 1; i <= numberOfProducts; i++)
                    {
                        productKeysToFetch.Add($"product:{200 + i}");
                    }
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

                    Thread.Sleep(10000);


                    Console.WriteLine("-------------------------------- Part 2: Bulk Async remove ---------------------------");
                    var productKeysToRemove = new List<string>();
                    for (int i = 1; i <= numberOfProducts; i++)
                    {
                        if (i % 2 == 0)
                            productKeysToRemove.Add($"product:{200 + i}");
                    }
                    // Bulk Remove
                    _cache.RemoveBulk(productKeysToRemove);
                    Console.WriteLine("Bulk removed products.");


                    Console.WriteLine("-------------------------------- Part 2: Bulk Get using a Loop ---------------------------");

                    Console.WriteLine("Preparing to retrieve all products using GetBulk...");

                    retrievedProducts = _cache.GetBulk<Product>(productKeysToFetch);

                    Console.WriteLine($"Successfully retrieved {retrievedProducts.Count} products from the cache.");

                    if (retrievedProducts.Count > 0)
                    {
                        Console.WriteLine("Retrieved Products After Removal:");
                        foreach (var kvp in retrievedProducts)
                        {
                            Console.WriteLine($"  -> Key: {kvp.Key}, Value: {kvp.Value}");
                        }
                    }
                    Thread.Sleep(10000);

                }
                else
                {
                    Console.WriteLine("Failed to connect to cache.");
                }
            }
            catch (OperationFailedException ex)
            {
                Console.WriteLine($"NCache Operation Failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
               // _cache.Clear();
                 //_cache?.Dispose();
                //Console.WriteLine("Cache cleared. Press any key to exit...");
            }

            Console.ReadKey();
        }
    
    }
}