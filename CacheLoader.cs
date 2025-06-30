using Alachisoft.NCache.Runtime.CacheLoader;
using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Providers;
namespace NcacheDemo.Providers
{
    public class CacheLoader : ICacheLoader
    {
        private SqlConnection _connection;
        private readonly object _dbLock = new object();

        // Establishes the database connection
        public void Init(IDictionary<string, string> parameters, string cacheName)
        {
            try
            {
                string connString = GetConnectionString(parameters);
                if (!string.IsNullOrEmpty(connString))
                {
                    _connection = new SqlConnection(connString);
                    _connection.Open();
                    Console.WriteLine($"CacheLoader for cache '{cacheName}': Database connection established.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CacheLoader Init Failed: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        // *** CORRECTED: This method now returns a Dictionary<string, ProviderCacheItem> ***
        public object LoadDatasetOnStartup(string dataSet)
        {
            if (string.IsNullOrEmpty(dataSet) || dataSet.ToLower() != "products")
            {
                return null;
            }

            // The method should return a dictionary where the key is the cache key.
            var itemsToLoad = new Dictionary<string, ProviderCacheItem>();
            string query = "SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products";

            try
            {
                lock (_dbLock)
                {
                    using (var command = new SqlCommand(query, _connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductProvider
                            {
                                Id = Convert.ToInt32(reader["ProductID"]),
                                Name = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["UnitPrice"]),
                                CategoryId = Convert.ToInt32(reader["CategoryID"]),
                                
                            };

                            // *** The KEY is now the key in the dictionary ***
                            string cacheKey = $"ProductProvider:{product.Id}";

                            var item = new ProviderCacheItem(product);
                            // Set ResyncOptions so NCache checks this dataset for updates
                            item.ResyncOptions = new ResyncOptions(true, dataSet);

                            // Add the key-value pair to the dictionary
                            itemsToLoad[cacheKey] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CacheLoader LoadDatasetOnStartup failed: {ex.Message}");
            }

            return itemsToLoad; // Return the dictionary
        }

        // *** CORRECTED: This method now returns a Dictionary<string, ProviderCacheItem> ***
        public object RefreshDataset(string dataSet, object userContext)
        {
            if (string.IsNullOrEmpty(dataSet) || dataSet.ToLower() != "products" || !(userContext is DateTime))
            {
                return null;
            }

            
            var itemsToRefresh = new Dictionary<string, ProviderCacheItem>();
            DateTime lastCheckTime = (DateTime)userContext;
            /*
            string query = "SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products WHERE LastModify > @LastCheckTime";

            try
            {
                lock (_dbLock)
                {
                    using (var command = new SqlCommand(query, _connection))
                    {
                        command.Parameters.Add("@LastCheckTime", SqlDbType.DateTime).Value = lastCheckTime;
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new ProductProvider
                                {
                                    Id = Convert.ToInt32(reader["ProductID"]),
                                    Name = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["UnitPrice"]),
                                    CategoryId = Convert.ToInt32(reader["CategoryID"]),
                                   
                                };

                                string cacheKey = $"ProductProvider:{product.Id}";

                                var item = new ProviderCacheItem(product);
                                item.ResyncOptions = new ResyncOptions(true, dataSet);

                                itemsToRefresh[cacheKey] = item;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CacheLoader RefreshDataset failed: {ex.Message}");
            }
            */
            return itemsToRefresh; // Return the dictionary
        }


        public IDictionary<string, RefreshPreference> GetDatasetsToRefresh(IDictionary<string, object> userContexts)
        {
            var datasetsToRefresh = new Dictionary<string, RefreshPreference>();
            /*
            foreach (var context in userContexts)
            {
                string dataSet = context.Key;
                if (dataSet.ToLower() != "products" || !(context.Value is DateTime)) continue;

                DateTime lastCheckTime = (DateTime)context.Value;
                string query = "SELECT 1 FROM Products WHERE LastModify > @LastCheckTime";

                try
                {
                    lock (_dbLock)
                    {
                        using (var command = new SqlCommand(query, _connection))
                        {
                            command.Parameters.Add("@LastCheckTime", SqlDbType.DateTime).Value = lastCheckTime;
                            if (command.ExecuteScalar() != null) // More efficient check
                            {
                                datasetsToRefresh.Add(dataSet, RefreshPreference.RefreshNow);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"CacheLoader GetDatasetsToRefresh failed: {ex.Message}");
                }
            }*/
            return datasetsToRefresh;
        }

        public object GetUserContext(string dataset)
        {
            return DateTime.UtcNow;
        }

        private string GetConnectionString(IDictionary<string, string> parameters)
        {
            if (parameters == null) return string.Empty;
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = parameters.ContainsKey("server") ? parameters["server"] : null,
                InitialCatalog = parameters.ContainsKey("database") ? parameters["database"] : null,
            };
            string username = parameters.ContainsKey("username") ? parameters["username"] : null;
            if (string.IsNullOrEmpty(username))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = username;
                builder.Password = parameters.ContainsKey("password") ? parameters["password"] : null;
            }
            return builder.ConnectionString;
        }
    }
}