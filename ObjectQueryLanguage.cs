using Alachisoft.NCache.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class ObjectQueryLanguage
    {
        public void Run(ICache _cache, string CacheName)
        {
            PopulateCache(_cache);
            RunAllOQLQueries(_cache);
            _cache.Dispose();
            Console.WriteLine("\nDisconnected from cache. Press any key to exit.");
            Console.ReadKey();
        }

        private void RunAllOQLQueries(ICache _cache)
        {
            Console.WriteLine("\nExecuting Where Query\n");
            WhereQuery(_cache,290);
            Console.WriteLine("\nExecuting Complex Query\n");
            ComplexQuery(_cache);
            Console.WriteLine("\nExecuting IN Query\n");
            INQuery(_cache);
            Console.WriteLine("\nExecuting Like Query\n");
            LikeQuery(_cache);
            Console.WriteLine("\nExecuting Delete Query\n");
            DeleteQuery(_cache);
            Console.WriteLine("\n After Delete Query\n");
            WhereQuery(_cache, 200);
        }
        public void ComplexQuery(ICache _cache)
        {
            string query = "SELECT Id, Name, Price FROM NcacheDemo.Product WHERE (Name LIKE ? AND Price > ?) ORDER BY Id DESC";

            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("Name", "Product #*");
            queryCommand.Parameters.Add("Price", 25.00);

            // queryCommand.Parameters.Add("Id", 290);

            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {

                    int id = reader.GetValue<int>("Id");
                    string name = reader.GetValue<string>("Name");
                    double price = reader.GetValue<double>("Price");
                    Console.WriteLine($"Product: {id} , Name: {name}, Unit Price: {price} \n");
                }
            }
            else
            {
                Console.WriteLine("NO Product found");
            }
        }

        public void WhereQuery(ICache _cache, int ID)
        {
            string query = "SELECT * FROM NcacheDemo.Product WHERE Id > ?";

            var queryCommand = new QueryCommand(query);
            
            queryCommand.Parameters.Add("Id", ID);
            
            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    
                    int id = reader.GetValue<int>("Id");
                    string name = reader.GetValue<string>("Name");
                    double price = reader.GetValue<double>("Price");
                    Console.WriteLine($"Product: {id} , Name: {name}, Unit Price: {price} \n");
                }
            }
            else
            {
                Console.WriteLine("NO Product found");
            }
        }


        public void INQuery(ICache _cache)
        {
            string query = "SELECT * FROM NcacheDemo.Product WHERE Price IN (?,?,?)";
            ArrayList unitsArray = new ArrayList();
            unitsArray.Add(19.99);
            unitsArray.Add(24.99);
            unitsArray.Add(42.49);
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("Price", unitsArray);



            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {

                    int id = reader.GetValue<int>("Id");
                    string name = reader.GetValue<string>("Name");
                    double price = reader.GetValue<double>("Price");
                    Console.WriteLine($"Product: {id} , Name: {name}, Unit Price: {price} \n");
                }
            }
            else
            {
                Console.WriteLine("NO Product found");
            }
        }


        public void LikeQuery(ICache _cache)
        {
            string query = "SELECT * FROM NcacheDemo.Product WHERE Name LIKE ?";

            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("Name", "Product #1");

            ICacheReader reader = _cache.SearchService.ExecuteReader(queryCommand);
            if (reader.FieldCount > 0)
            {
                while (reader.Read())
                {

                    int id = reader.GetValue<int>("Id");
                    string name = reader.GetValue<string>("Name");
                    double price = reader.GetValue<double>("Price");
                    Console.WriteLine($"Product: {id} , Name: {name}, Unit Price: {price} \n");
                }
            }
            else
            {
                Console.WriteLine("NO Product found");
            }
        }


        public void DeleteQuery(ICache _cache)
        {
            string query = "DELETE FROM  NcacheDemo.Product WHERE Id > ?";
            var queryCommand = new QueryCommand(query);
            queryCommand.Parameters.Add("Id", 250);
            _cache.SearchService.ExecuteNonQuery(queryCommand);
        }
        private void PopulateCache(ICache _cache)
        {
            var productItems = new Dictionary<string, CacheItem>();
            for (int i = 1; i <= 100; i++)
            {
                var product = new Product
                {
                    Id = 200 + i,
                    Name = $"Product #{i}",
                    Price = Math.Round(19.99 + (i * 2.5), 2)
                };

               
                string cacheKey = $"product:{product.Id}";
                productItems.Add(cacheKey, new CacheItem(product));
            }

           
            _cache.InsertBulk(productItems);
            Console.WriteLine("Population complete.");
        }
    }
}
