using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NcacheDemo;

namespace NcacheDemo
{
    public class ContinuousQueryClass
    {
        public void OnChangeInQueryResultSet(string key, CQEventArg arg)
        {
            switch (arg.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine($"Item with key '{key}' has been added to result set of continuous query");
                    break;

                case EventType.ItemUpdated:
                    Console.WriteLine($"Item with key '{key}' has been updated in the result set of continuous query");

                    if (arg.Item != null)
                    {
                        Product updatedProduct = arg.Item.GetValue<Product>();
                        Console.WriteLine($"Updated product '{updatedProduct.Name}' with key '{key}' has ID '{updatedProduct.Id}'");
                    }
                    break;

                case EventType.ItemRemoved:
                    Console.WriteLine($"Item with key '{key}' has been removed from result set of continuous query");
                    break;
            }
        }


        public void Run(ICache _cache)
        {
            _cache.Clear();

            Console.WriteLine("Cache connected and cleared.");

          
            string query = "SELECT $VALUE$ FROM NcacheDemo.Product WHERE Category = ?";

            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("Category", "Beverages");

            // Create the Continuous Query
            var cQuery = new ContinuousQuery(queryCommand);

             cQuery.RegisterNotification(
                OnChangeInQueryResultSet,
                EventType.ItemAdded | EventType.ItemUpdated | EventType.ItemRemoved,
                EventDataFilter.DataWithMetadata);

            
            _cache.MessagingService.RegisterCQ(cQuery);
            Console.WriteLine("Continuous Query registered successfully for 'Beverages' category.");
            Console.WriteLine("----------------------------------------------------------\n");

          
            Console.WriteLine("\nAction: Adding a 'Beverage' product (key: Product:1)...");
            var p1 = new Product { Id = 1, Name = "Green Tea", Category = "Beverages", Price = 5.99 };
            _cache.Add($"Product:{p1.Id}", p1);
            Thread.Sleep(1000); // Wait for notification

           
            Console.WriteLine("\nAction: Adding a 'Dairy' product (key: Product:2)...");
            var p2 = new Product { Id = 2, Name = "Milk", Category = "Dairy", Price = 3.49 };
            _cache.Add($"Product:{p2.Id}", p2);
            Thread.Sleep(1000);

            
            Console.WriteLine("\nAction: Updating the 'Beverage' product (key: Product:1)...");
            p1.Price = 6.49; 
            _cache.Insert($"Product:{p1.Id}", p1);
            Thread.Sleep(1000);

           
            Console.WriteLine("\nAction: Updating the 'Dairy' product to become a 'Beverage' (key: Product:2)...");
            p2.Category = "Beverages";
            p2.Name = "Chocolate Milk";
            _cache.Insert($"Product:{p2.Id}", p2);
            Thread.Sleep(1000);

            
            Console.WriteLine("\nAction: Updating 'Green Tea' to be 'Produce' (key: Product:1)...");
            p1.Category = "Produce";
            p1.Name = "Organic Green Tea Leaves";
            _cache.Insert($"Product:{p1.Id}", p1);
            Thread.Sleep(1000);

            
            Console.WriteLine("\nAction: Removing 'Chocolate Milk' (key: Product:2)...");
            _cache.Remove($"Product:{p2.Id}");
            Thread.Sleep(1000);

            
            _cache.MessagingService.UnRegisterCQ(cQuery);
            Console.WriteLine("\nContinuous Query unregistered.");

            // Dispose the cache handle
            _cache.Dispose();
        }
    }
}
