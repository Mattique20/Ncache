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
        //private static ICache _cache;
        private const string CacheName = "59PORClusteredCache"; 

        static void Main(string[] args)
        {
            try
            { 
                Console.WriteLine("NCache Bulk Operations Demo");


                ICache cache = CacheManager.GetCache(CacheName);
                Console.WriteLine($"Connected to cache");
                cache.Clear(); 

                var itemLevelEventsDemo = new ItemLevelEvent();

                // --- Step 1: Add an item with a specific notification attached ---
                Console.WriteLine("\n--- STEP 1: ADDING ITEM WITH NOTIFICATION ---");
                var product = new Product { Id = 101, Name = "Laptop", Price = 1200 };
                var product2 = new Product { Id = 102, Name = "GPU", Price = 1200 };
                var product3 = new Product { Id = 103, Name = "RAM", Price = 1200 };
                itemLevelEventsDemo.AddItemWithSpecificNotification(cache, product);

                var cacheItem = new CacheItem(product);
                var cacheItem2 = new CacheItem(product2);
                var cacheItem3 = new CacheItem(product3);

            
                cache.Insert($"product:{product2.Id}", cacheItem2);
                cache.Insert($"product:{product3.Id}", cacheItem3);
                Console.WriteLine("items inserted WITHOUT a notification.");

                cache.Insert($"product:{product.Id}", cacheItem);
                // --- Step 2: Update the item to trigger the 'ItemUpdated' event ---
                Console.WriteLine("\n--- STEP 2: UPDATING THE ITEM TO TRIGGER EVENT ---");
                Console.WriteLine("Waiting 2 seconds before update...");
                Thread.Sleep(2000); 

                // Create the updated version of the product
                var updatedProduct = new Product { Id = 101, Name = "Gaming Laptop", Price = 1499 };
                cache.Insert($"product:{product.Id}", updatedProduct); 
                Console.WriteLine($"--> Updated item 'product:{product.Id}' in the cache.");

                // Also update the other item - notice no event will fire for this one.
                var updatedProduct2 = new Product { Id = 102, Name = "GPU", Price = 11499 };
                cache.Insert($"product:{product2.Id}", updatedProduct2);
                Console.WriteLine($"--> Updated item 'product:{product2.Id}' in the cache.");


                // --- Step 3: Remove the item to trigger the 'ItemRemoved' event ---
                Console.WriteLine("\n--- STEP 3: REMOVING THE ITEM TO TRIGGER EVENT ---");
                Console.WriteLine("Waiting 5 seconds before removal...");
                Thread.Sleep(100); 

                cache.Remove($"product:{product.Id}");
                Console.WriteLine($"--> Removed item 'product:{product.Id}' from the cache.");

                Console.WriteLine("\nDemo finished. Press any key to exit.");
                Console.ReadKey();
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