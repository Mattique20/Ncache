// Events.cs
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using System;

namespace NcacheDemo
{
    public class Events
    {
        public void OnCacheDataModification(string key, CacheEventArg args)
        {
            switch (args.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine($"\n[EVENT] ==> Item Added. Key: '{key}', Cache: '{args.CacheName}'");
                    break;

                case EventType.ItemUpdated:
                    Console.WriteLine($"\n[EVENT] ==> Item Updated. Key: '{key}', Cache: '{args.CacheName}'");

                    // This block will now execute because we changed the EventDataFilter
                    if (args.Item != null)
                    {
                        // Safely get the value from the CacheItem
                        Product updatedProduct = args.Item.GetValue<Product>();
                        if (updatedProduct != null)
                        {
                            Console.WriteLine($"L-- Updated product details: Name='{updatedProduct.Name}', Price='{updatedProduct.Price}'");
                        }
                    }
                    break;

                case EventType.ItemRemoved:
                    Console.WriteLine($"\n[EVENT] ==> Item Removed. Key: '{key}', Cache: '{args.CacheName}'");
                    break;
            }
        }

        public void RegisterCacheNotificationsForAllOperations(ICache cache)
        {
            var dataNotificationCallback = new CacheDataNotificationCallback(OnCacheDataModification);

            // CRITICAL CHANGE HERE: Use DataWithMetadata to get the item back in the event argument
            CacheEventDescriptor eventDescriptor = cache.MessagingService.RegisterCacheNotification(
                dataNotificationCallback,
                EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved,
                EventDataFilter.None // Changed from .None
            );

            if (eventDescriptor.IsRegistered)
            {
                Console.WriteLine("Cache level notifications registered successfully.");
            }
        }

        public void AddItemWithSpecificNotification(ICache cache, Product product)
        {
            string key = $"product:{product.Id}";
            var cacheItem = new CacheItem(product);

            // 1. Define the callback that will be triggered for THIS ITEM ONLY.
            CacheDataNotificationCallback itemSpecificCallback = (itemKey, args) =>
            {
                Console.WriteLine($"\n[ITEM-LEVEL EVENT] ==> The specific item '{itemKey}' was {args.EventType}!");
            };

            // 2. Attach the notification directly to the CacheItem.
            // This says "notify me only if THIS item is updated or removed".
            cacheItem.SetCacheDataNotification(itemSpecificCallback, EventType.ItemUpdated | EventType.ItemRemoved, EventDataFilter.None);

            // 3. Insert the item into the cache. The event registration travels with it.
            cache.Insert(key, cacheItem);

            Console.WriteLine($"Item '{key}' inserted with a specific item-level notification attached.");
        }
    }
}