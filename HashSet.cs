using Alachisoft.NCache.Client;
using Alachisoft.NCache.Client.DataTypes;
using Alachisoft.NCache.Client.DataTypes.Collections;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Dependencies;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NcacheDemo
{
    public class HashSet
    {
        private void DataTypeDataNotificationCallback(string collectionName, DataTypeEventArg collectionEventArgs)
        {
            switch (collectionEventArgs.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine($"[Event Triggered] by {collectionName} item has been added");
                    break;
                case EventType.ItemUpdated:
                    if (collectionEventArgs.CollectionItem != null)
                    {
                        Console.WriteLine($"[Event Triggered] by {collectionName} item has been Updated");
                    }
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine($"[Event Triggered] by {collectionName} item has been Removed");
                    break;
            }
        }
        public void Run(ICache cache)
        {


            /*
            var attributes = new DataTypeAttributes();
            // group 
            attributes.Group = "Electronics";
            //attributes 

            attributes.Priority = CacheItemPriority.High;
            // Absolute expiration of 15 minutes
            var expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromMinutes(15));
            attributes.Expiration = expiration;
            // key dependency of orderKey on customerKey
            string productKey = "Product:101";
             var keyDependency = new KeyDependency(productKey);
            attributes.Dependency = keyDependency;
            //tags
            Tag[] tags = new Tag[2];
            tags[0] = new Tag("2 Year Warranty");
            tags[1] = new Tag("Stainless Steel");
            attributes.Tags = tags;
            //named tags
            var namedTag = new NamedTagsDictionary();
            namedTag.Add("Discount", 0.4);
            attributes.NamedTags = namedTag;
            */
            //-----------------------------------Monday User
            string mondayUsersKey = "MondayUsers";
            IDistributedHashSet<int> userSetMonday = cache.DataTypeManager.CreateHashSet<int>(mondayUsersKey);
            
            userSetMonday.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
            EventType.ItemUpdated | EventType.ItemRemoved,
            DataTypeEventDataFilter.Data);
            
            userSetMonday.Add(1223);
            userSetMonday.Add(34564);
            userSetMonday.Add(3564);

            Thread.Sleep(1000);
            //-----------------------------------Tuesday User
            string tuesdayUsersKey = "TuesdayUsers";
            IDistributedHashSet<int> userSetTuesday = cache.DataTypeManager.CreateHashSet<int>(tuesdayUsersKey);

            userSetTuesday.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
            EventType.ItemUpdated | EventType.ItemRemoved,
            DataTypeEventDataFilter.Data);
            
            userSetTuesday.Add(4545);
            userSetTuesday.Add(34564);
            userSetTuesday.Add(3564);
            userSetTuesday.Add(7879);
            Thread.Sleep(1000);


            //-----------------------------------Total User
            string totalUsersKey = "TotalUsers";
            IDistributedHashSet<int> userSetTotal = cache.DataTypeManager.CreateHashSet<int>(totalUsersKey);
            userSetTotal.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
            EventType.ItemUpdated | EventType.ItemRemoved,
            DataTypeEventDataFilter.Data);


            //-----------------------------------weekly User
            string weeklyUsersKey = "WeeklyUsers";
            IDistributedHashSet<int> userSetWeekly = cache.DataTypeManager.CreateHashSet<int>(weeklyUsersKey);
            userSetWeekly.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
           EventType.ItemUpdated | EventType.ItemRemoved,
           DataTypeEventDataFilter.Data);


            //-----------------------------------Diff User
            string differenceKey = "Difference";
            IDistributedHashSet<int> userSetDifference = cache.DataTypeManager.CreateHashSet<int>(differenceKey);
            userSetDifference.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
           EventType.ItemUpdated | EventType.ItemRemoved,
           DataTypeEventDataFilter.Data);

            
            userSetMonday.StoreUnion(totalUsersKey, tuesdayUsersKey);
            Thread.Sleep(1000);

            userSetMonday.StoreIntersection(weeklyUsersKey, tuesdayUsersKey);
            Thread.Sleep(1000);

            userSetMonday.StoreDifference(differenceKey, tuesdayUsersKey);
            Thread.Sleep(1000);
        }
    }
}
