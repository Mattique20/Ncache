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
                case EventType.ItemAdded:
                    Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> Item Added!");
                    Console.WriteLine($"L-- Key: '{key}', Cache: '{args.CacheName}'");
                    break;

                case EventType.ItemUpdated:
                    Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> Item Updated!");
                    Console.WriteLine($"L-- Key: '{key}', Cache: '{args.CacheName}'");
                    if (args.Item != null)
                    {
                        Product updatedProduct = args.Item.GetValue<Product>();
                        if (updatedProduct != null)
                        {
                            Console.WriteLine($"L-- Updated product details: {updatedProduct}");
                        }
                    }
                    break;

                case EventType.ItemRemoved:
                    Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> Item Removed!");
                    Console.WriteLine($"L-- Key: '{key}', Cache: '{args.CacheName}'");
                    break;
            }
        }

        public void run(String CacheName, ICache _cache)
        {
            // The passed-in _cache is not used, so let's stick to getting it here
            ICache cache = CacheManager.GetCache(CacheName);
            Console.WriteLine($"Connected to cache '{CacheName}'.");
            cache.Clear();

            // --- Define the callback once for reuse ---
            var itemSpecificCallback = new CacheDataNotificationCallback(OnSpecificItemModified);
            var eventType = EventType.ItemUpdated | EventType.ItemRemoved | EventType.ItemAdded;
            var dataFilter = EventDataFilter.DataWithMetadata;

            // --- Step 1: Add an item with a specific notification attached ---
            Console.WriteLine("\n--- STEP 1: ADDING ITEM WITH NOTIFICATION ---");
            var product = new Product { Id = 101, Name = "Laptop", Price = 1200 };
            string key = $"product:{product.Id}";

            var cacheItem = new CacheItem(product);
            cacheItem.SetCacheDataNotification(itemSpecificCallback, eventType, dataFilter);

            cache.Insert(key, cacheItem);
            Console.WriteLine($"--> Inserted item '{key}' with an event notification.");

            // Also add an item WITHOUT a notification to prove it doesn't fire events
            var product2 = new Product { Id = 102, Name = "GPU", Price = 800 };
            cache.Insert($"product:{product2.Id}", product2);
            Console.WriteLine($"--> Inserted item 'product:{product2.Id}' WITHOUT a notification.");


            // --- Step 2: Update the item to trigger the 'ItemUpdated' event ---
            Console.WriteLine("\n--- STEP 2: UPDATING THE ITEM TO TRIGGER EVENT ---");
            var updatedProduct = new Product { Id = 101, Name = "Gaming Laptop", Price = 1499.99 };

            // *** CHANGE 1: Create a NEW CacheItem and re-apply the notification ***
            // This ensures the event registration persists after the update.
            var updatedCacheItem = new CacheItem(updatedProduct);
            updatedCacheItem.SetCacheDataNotification(itemSpecificCallback, eventType, dataFilter);

            cache.Insert(key, updatedCacheItem);
            Console.WriteLine($"--> Updated item '{key}' in the cache.");

            // Update the other item - no event will fire for this one.
            var updatedProduct2 = new Product { Id = 102, Name = "GPU (New Gen)", Price = 899 };
            cache.Insert($"product:{product2.Id}", updatedProduct2);
            Console.WriteLine($"--> Updated item 'product:{product2.Id}' (no event expected).");


            // --- Step 3: Remove the item to trigger the 'ItemRemoved' event ---
            Console.WriteLine("\n--- STEP 3: REMOVING THE ITEM TO TRIGGER EVENT ---");
            cache.Remove(key);
            Console.WriteLine($"--> Removed item '{key}' from the cache.");

            // *** CHANGE 2: Wait for events to arrive ***
            // Give the application time to receive and process the asynchronous notifications from the server.
            Console.WriteLine("\n--- WAITING FOR 5 SECONDS TO RECEIVE EVENTS... ---");
            Thread.Sleep(5000);

            Console.WriteLine("\nDemo finished. Press any key to exit.");
            Console.ReadKey();

            // It's good practice to dispose of the cache handle
            cache.Dispose();
        }
    }
}
