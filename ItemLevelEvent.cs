// Events.cs
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Threading;

namespace NcacheDemo
{
    public class ItemLevelEvent
    {
        public void OnSpecificItemModified(string key, CacheEventArg args)
        {
            switch (args.EventType)
            {

                case EventType.ItemUpdated:
                    Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> Item Updated!");
                    Console.WriteLine($"L-- Key: '{key}', Cache: '{args.CacheName}'");

                    // This block executes because we will use EventDataFilter.DataWithMetadata
                    if (args.Item != null)
                    {
                        // Safely get the value from the CacheItem
                        Product updatedProduct = args.Item.GetValue<Product>();
                        if (updatedProduct != null)
                        {
                            Console.WriteLine($"L-- Updated product details: {updatedProduct}");
                        }
                    }
                    break;

                case EventType.ItemRemoved:
                    // For remove events, the item's data is not typically sent back.
                    Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> Item Removed!");
                    Console.WriteLine($"L-- Key: '{key}', Cache: '{args.CacheName}'");
                    break;
            }
        }
        public void AddItemWithSpecificNotification(ICache cache, Product product)
        {
            string key = $"product:{product.Id}";
            var cacheItem = new CacheItem(product);
            var itemSpecificCallback = new CacheDataNotificationCallback(OnSpecificItemModified);
            cacheItem.SetCacheDataNotification(
                itemSpecificCallback,
                EventType.ItemUpdated | EventType.ItemRemoved,
                EventDataFilter.DataWithMetadata);
            cache.Insert(key, cacheItem);

            Console.WriteLine($"--> Item '{key}' inserted.");
            Console.WriteLine("L-- A specific notification for Update/Remove has been attached to it.");
        }



        public void run(String CacheName, ICache _cache)
        {
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
    }
}
