using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using Alachisoft.NCache.Runtime.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class Eviction
    {

        public void OnCacheDataModification(string key, CacheEventArg args)
        {
            switch (args.EventType)
            {

                case EventType.ItemRemoved:
                    Console.WriteLine($"\n[EVENT] ==> Item Removed. Key: '{key}', Cache: '{args.CacheName}'");
                    break;
            }
        }

        public void RegisterCacheNotificationsForAllOperations(ICache cache)
        {
            var dataNotificationCallback = new CacheDataNotificationCallback(OnCacheDataModification);
            CacheEventDescriptor eventDescriptor = cache.MessagingService.RegisterCacheNotification(
                dataNotificationCallback,
                 EventType.ItemRemoved,
                EventDataFilter.DataWithMetadata// Changed from .None
            );

            if (eventDescriptor.IsRegistered)
            {
                Console.WriteLine("Cache level notifications registered successfully.");
            }
        }
        public void Run(ICache _cache)
        {

           
            RegisterCacheNotificationsForAllOperations(_cache);


            var productItems = new Dictionary<string, CacheItem>();
            for (int i = 1; i <= 10000000; i++)
            {
                var product = new Product
                {
                    Id = 200 + i,
                    Name = $"Product #{i}",
                    Price = Math.Round(19.99 + (i * 2.5), 2)
                };
                string cacheKey = $"product:{product.Id}";
                var cacheItem1 = new CacheItem(product);
                _cache.Insert(cacheKey, cacheItem1);
            }


           // _cache.InsertBulk(productItems);
            Console.WriteLine("Population complete.");
        }
    }
}
