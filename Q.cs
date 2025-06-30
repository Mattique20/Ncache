using Alachisoft.NCache.Client;
using Alachisoft.NCache.Client.DataTypes.Collections;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NcacheDemo
{
    public class Q
    {
        private void DataTypeDataNotificationCallback(string collectionName, DataTypeEventArg collectionEventArgs)
        {
            switch (collectionEventArgs.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("[EVENTS TRIGGERED} Q has been created on server Side");
                    break;
                case EventType.ItemUpdated:
                    if (collectionEventArgs.CollectionItem != null)
                    {
                        Console.WriteLine("[EVENTS TRIGGERED} Q has been Updated on server Side");
                    }
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("[EVENTS TRIGGERED} Q has been removed on server Side");
                    break;
            }
        }

        public void Run(ICache _cache)
        {
            IDistributedQueue<Product> q = null;
            try
            {


                string key = "ProductQ";
                Console.WriteLine($"Registering event notification for Q");

                q = _cache.DataTypeManager.CreateQueue<Product>(key);
                q.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
                    EventType.ItemUpdated | EventType.ItemRemoved,
                    DataTypeEventDataFilter.Data);

                Console.WriteLine($"Distributed Q with key '{key}' created.");
                Thread.Sleep(1000);


                Console.WriteLine("\n-> Adding 3 products to the q...");
                var productsToAdd = new List<Product>
                {
                    new Product { Id = 101, Name = "Espresso", Category = "Coffee", Price = 2.99 },
                    new Product { Id = 102, Name = "Latte", Category = "Coffee", Price = 3.99 },
                    new Product { Id = 103, Name = "Orange Juice", Category = "Juice", Price = 4.50 }
                };
                q.EnqueueBulk(productsToAdd);
                Thread.Sleep(1000);

                IDistributedQueue<Product> retrievedQ = _cache.DataTypeManager.GetQueue<Product>(key);

                if (retrievedQ != null)
                {
                    foreach (var item in retrievedQ)
                    {
                        Console.WriteLine($"Item retrieved {item.Name} from q on server");
                    }
                }
                else
                {
                    Console.WriteLine($"No q exists");
                }

                Console.WriteLine("\n-> Updating the price of 'Latte'...");
                var tempStorage = new List<Product>();
                for (int i = 0; i < q.Count(); i++)
                {
                    
                    Product currentItem = q.Dequeue();

                    
                    if (currentItem.Name == "Latte")
                    {
                        Console.WriteLine($"Found 'Latte'. Old price: ${currentItem.Price:F2}. Updating price to $4.25.");
                        currentItem.Price = 4.25; // 3. Modify it
                    }

                    tempStorage.Add(currentItem);
                }
                Thread.Sleep(1000);
                q.EnqueueBulk(tempStorage);

                Console.WriteLine("\n-> Removing 'Espresso' from the q...");
                 q.Dequeue();
                 Thread.Sleep(1000);

                IDistributedQueue<Product> retrievedQ2 = _cache.DataTypeManager.GetQueue<Product>(key);

                Console.WriteLine($"\n-> Final items in q ({retrievedQ2.Count}):");
                foreach (var product in retrievedQ2)
                {
                    Console.WriteLine($"   - {product}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {

                if (q != null)
                {
                    q.UnRegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved);
                    Console.WriteLine("\nUnregistered event notifications.");
                }
                if (_cache != null)
                {
                    _cache.Dispose();
                    Console.WriteLine("Cache handle disposed.");
                }
            }
        }
    }
}
