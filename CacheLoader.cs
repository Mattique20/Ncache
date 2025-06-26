using Alachisoft.NCache.Runtime.CacheLoader;
using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
namespace Providers
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
                // This helper method builds the connection string from parameters
                var builder = new SqlConnectionStringBuilder();

                // Use case-insensitive check for parameter names
                string GetParam(string key)
                {
                    foreach (var k in parameters.Keys)
                    {
                        if (string.Equals(k.ToString(), key, StringComparison.OrdinalIgnoreCase))
                        {
                            return parameters[k] as string;
                        }
                    }
                    return null;
                }

                builder.DataSource = GetParam("server");
                builder.InitialCatalog = GetParam("database");
                string username = GetParam("username");

                if (string.IsNullOrEmpty(builder.DataSource) || string.IsNullOrEmpty(builder.InitialCatalog))
                {
                    throw new Exception("Required parameters 'server' and 'database' are missing.");
                }

                if (string.IsNullOrEmpty(username))
                {
                    builder.IntegratedSecurity = true;
                }
                else
                {
                    builder.UserID = username;
                    builder.Password = GetParam("password");
                }

                string connString = builder.ConnectionString;
                _connection = new SqlConnection(connString);
                _connection.Open(); // This is the line that is likely failing.
            }
            catch (Exception ex)
            {
                // THIS IS THE MOST IMPORTANT PART.
                // It wraps the real error and throws it, which will stop the cache from starting
                // and log the real database error (e.g., "Login failed for user 'sa'").
                throw new Exception($"Provider initialization failed. Please check NCache error logs for details. Original Error: {ex.Message}", ex);
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

            // *** CHANGE THIS: Use Hashtable instead of Dictionary ***
            var itemsToLoad = new Hashtable();
            string query = "SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products";

            try
            {
                // The lock and DB logic remains exactly the same
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

                            string cacheKey = $"ProductProvider:{product.Id}";
                            var item = new ProviderCacheItem(product);
                            item.ResyncOptions = new ResyncOptions(true, dataSet);

                            // Add to the Hashtable. The syntax is the same.
                            itemsToLoad[cacheKey] = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // This is still good to have for debugging
                Console.WriteLine($"CacheLoader LoadDatasetOnStartup failed: {ex.Message}");
            }

            // Return the Hashtable
            return itemsToLoad;
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