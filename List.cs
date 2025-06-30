using Alachisoft.NCache.Client;
using Alachisoft.NCache.Client.DataTypes.Collections;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NcacheDemo { 
    class List
    {
        private void DataTypeDataNotificationCallback(string collectionName, DataTypeEventArg collectionEventArgs)
        {
            switch (collectionEventArgs.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("[EVENTS TRIGGERED} List has been created on server Side");
                    break;
                case EventType.ItemUpdated:
                    if (collectionEventArgs.CollectionItem != null)
                    {
                        Console.WriteLine("[EVENTS TRIGGERED} List has been Updated on server Side");
                    }
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("[EVENTS TRIGGERED} List has been removed on server Side");
                    break;
            }
        }

        public void Run(ICache _cache)
        {
            IDistributedList<Product> list = null;
            try
            {
               

                string key = "ProductList";
                Console.WriteLine($"Registering event notification for List");
               
                list = _cache.DataTypeManager.CreateList<Product>(key);
                list.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
                    EventType.ItemUpdated | EventType.ItemRemoved,
                    DataTypeEventDataFilter.Data);

                Console.WriteLine($"Distributed List with key '{key}' created.");
                Thread.Sleep(1000);
                

                Console.WriteLine("\n-> Adding 3 products to the list...");
                var productsToAdd = new List<Product>
                {
                    new Product { Id = 101, Name = "Espresso", Category = "Coffee", Price = 2.99 },
                    new Product { Id = 102, Name = "Latte", Category = "Coffee", Price = 3.99 },
                    new Product { Id = 103, Name = "Orange Juice", Category = "Juice", Price = 4.50 }
                };
                list.AddRange(productsToAdd);
                Thread.Sleep(1000);

                IDistributedList<Product> retrievedList = _cache.DataTypeManager.GetList<Product>(key);

                if (retrievedList != null)
                {
                    foreach (var item in retrievedList)
                    {
                        Console.WriteLine($"Item retrieved {item.Name} from list on server");
                    }
                }
                else
                {
                    Console.WriteLine($"No list exists");
                }

                Console.WriteLine("\n-> Updating the price of 'Latte'...");
                Product latte = list[1]; 
                latte.Price = 4.25;      
                list[1] = latte;         
                Thread.Sleep(1000);

               
                Console.WriteLine("\n-> Removing 'Espresso' from the list...");
                list.RemoveAt(0); 
                Thread.Sleep(1000);

                IDistributedList<Product> retrievedList2 = _cache.DataTypeManager.GetList<Product>(key);

                Console.WriteLine($"\n-> Final items in list ({retrievedList2.Count}):");
                foreach (var product in retrievedList2)
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
                
                if (list != null)
                {
                    list.UnRegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved);
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
