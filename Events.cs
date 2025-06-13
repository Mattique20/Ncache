using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    public class Events
    {
        public Events() { }
        public void OnCacheDataModification(string key, CacheEventArg args)
        {
            switch (args.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine($"Item with Key '{key}' has been added to cache '{args.CacheName}'");
                    break;

                case EventType.ItemUpdated:
                    Console.WriteLine($"Item with Key '{key}' has been updated in the cache '{args.CacheName}'");

                    // Item can be used if EventDataFilter is DataWithMetadata or Metadata
                    if (args.Item != null)
                    {
                        Product updatedProduct = args.Item.GetValue<Product>();
                        Console.WriteLine($"Updated Item is a Product having name '{updatedProduct.Name}', price '{updatedProduct.Price}' ");
                    }
                    break;

                case EventType.ItemRemoved:
                    Console.WriteLine($"Item with Key '{key}' has been removed from the cache '{args.CacheName}'");
                    break;
            }
        }

        public void RegisterCacheNotificationsForAllOperations(ICache cache)
        {
            // create CacheDataNotificationCallback object
            var dataNotificationCallback = new CacheDataNotificationCallback(OnCacheDataModification);

            // Register cache notification with "ItemAdded" EventType and
            // EventDataFilter "None" which means only keys will be returned
            CacheEventDescriptor eventDescriptor = cache.MessagingService.RegisterCacheNotification(dataNotificationCallback, EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved, EventDataFilter.None);

            if (eventDescriptor.IsRegistered)
            {
                Console.WriteLine("Cache level notifications registered successfully");
            }
        }
    }
}
