using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Client.DataTypes.Collections;
using Alachisoft.NCache.Client.DataTypes.Counter;
using Alachisoft.NCache.Runtime.Events;
namespace NcacheDemo
{
    public class Counter
    {
        private void DataTypeDataNotificationCallback(string collectionName, DataTypeEventArg collectionEventArgs)
        {
            switch (collectionEventArgs.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("[EVENTS TRIGGERED} Counter has been created on server Side");
                    break;
                case EventType.ItemUpdated:
                    if (collectionEventArgs.CollectionItem != null)
                    {
                        Console.WriteLine("[EVENTS TRIGGERED} Counter has been Updated on server Side");
                    }
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("[EVENTS TRIGGERED} Counter has been removed on server Side");
                    break;
            }
        }


        public void Run(ICache cache)
        {
            string key = "SubscriptionCounter";
            long initialValue = 15;
            ICounter counter = cache.DataTypeManager.CreateCounter(key, initialValue);
            counter.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
        EventType.ItemUpdated | EventType.ItemRemoved,
        DataTypeEventDataFilter.Data);


            ICounter retrievedCounter = cache.DataTypeManager.GetCounter(key);
            retrievedCounter.SetValue(100);
            long newValue = retrievedCounter.Increment();
            newValue = retrievedCounter.Decrement();
            newValue = retrievedCounter.IncrementBy(10);
            newValue = retrievedCounter.DecrementBy(5);
        }
    }
}
