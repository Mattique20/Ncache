using Alachisoft.NCache.Client;
using Alachisoft.NCache.Client.DataTypes;
using Alachisoft.NCache.Client.DataTypes.Collections;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NcacheDemo
{
    public class Dictionary
    {
        private void DataTypeDataNotificationCallback(string collectionName, DataTypeEventArg collectionEventArgs)
        {
            switch (collectionEventArgs.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("[EVENTS TRIGGERED} Dictionary has been created on server Side");
                    break;
                case EventType.ItemUpdated:
                    if (collectionEventArgs.CollectionItem != null)
                    {
                        Console.WriteLine("[EVENTS TRIGGERED} Dictionary has been Updated on server Side");
                    }
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("[EVENTS TRIGGERED} Dictionary has been removed on server Side");
                    break;
            }
        }

        public void Run(ICache cache)
        {
            string key = "ProductDictionary";
            Console.WriteLine($"Registering event notification for Q");
            
            var attributes = new DataTypeAttributes();
            attributes.Priority = CacheItemPriority.High;
            var expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromMinutes(15));
            attributes.Expiration = expiration;

            IDistributedDictionary<string, Product> dictionary = cache.DataTypeManager.CreateDictionary<string, Product>(key,attributes);
            dictionary.RegisterNotification(DataTypeDataNotificationCallback, EventType.ItemAdded |
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

            foreach (var product in productsToAdd)
            {
                var p = new CacheItem(product);
                
                p.Expiration = expiration;

                string productKey = $"Product:{product.Id}";
                dictionary.Add(productKey, product);
                Thread.Sleep(1000);
            }
            IDistributedDictionary<string, Product> retrievedDictionary = cache.DataTypeManager.GetDictionary<string, Product>(key);
            if (retrievedDictionary != null)
            {
                foreach (var item in retrievedDictionary)
                {
                    Console.WriteLine($" {item.Key} with value {item.Value} retrived");
                }
            }
            else
            {
                Console.WriteLine($" No Dictionary Exists");
            }


            Console.WriteLine("------------------Imserting new items------------------");
            var newProductsToAdd = new List<Product>
            {
                new Product { Id = 104, Name = "Coke", Category = "SoftDrink", Price = 2.99 },
                new Product { Id = 105, Name = "Pepsi", Category = "SoftDrink", Price = 3.99 },
                new Product { Id = 106, Name = "Pineapple Juice", Category = "Juice", Price = 4.50 }
            };
            
            IDictionary<string, Product> newProducts = new Dictionary<string, Product>();
            foreach (var product in newProductsToAdd)
            {
                string productKey = $"Product:{product.Id}";
                newProducts.Add(productKey, product);
            }

            retrievedDictionary.Insert(newProducts);
            var keysToRemove = new List<string>();
            keysToRemove.Add("Product:101");
            keysToRemove.Add("Product:102");
            keysToRemove.Add("Product:103");

            retrievedDictionary.Insert(newProducts);

            Thread.Sleep(1000);
            
            
            
            int itemsRemoved = retrievedDictionary.Remove(keysToRemove);
            Thread.Sleep(1000);


            Console.WriteLine("------------------After removal------------------");
            IDistributedDictionary<string, Product> retrievedDictionary2 = cache.DataTypeManager.GetDictionary<string, Product>(key);
            if (retrievedDictionary2 != null)
            {
                foreach (var item in retrievedDictionary2)
                {
                    Console.WriteLine($" {item.Key} with value {item.Value} retrived");
                }
            }
            else
            {
                Console.WriteLine($" No Dictionary Exists");
            }
        }
    }
}
