using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo.SampleData
{
    class MockDB
    {
       
            public static List<Product> Products { get; private set; }
            public static List<Supplier> Suppliers { get; private set; }

            // Static constructor to pre-populate our "database" with initial data
            static MockDB()
            {
                var now = DateTime.UtcNow;
                Products = new List<Product>
            {
                new Product { Id = 1, Name = "Chai", Price = 18 ,LastModify = now.AddHours(-1) },
                new Product { Id = 2, Name = "Chang", Price = 19, LastModify = now.AddHours(-2) },
                new Product { Id = 3, Name = "Aniseed Syrup", Price = 10, LastModify = now.AddHours(-1) }
            };

                Suppliers = new List<Supplier>
            {
                new Supplier { SupplierId = 101, CompanyName = "Exotic Liquids", ContactName = "Charlotte Cooper", LastModify = now.AddMinutes(-30) },
                new Supplier { SupplierId = 102, CompanyName = "New Orleans Cajun Delights", ContactName = "Shelley Burke",LastModify = now.AddMinutes(-45)  }
            };
            }

            // A helper method to simulate a database update
            public static void UpdateRandomProductPrice()
            {
                if (Products.Any())
                {
                    var random = new Random();
                    var productToUpdate = Products[random.Next(Products.Count)];
                    productToUpdate.Price += 1;
                    Console.WriteLine($"MOCK DB: Updated {productToUpdate.Name} price to {productToUpdate.Price}.");
                }
            }
    }
}


