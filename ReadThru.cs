using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.DatasourceProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Providers
{
    public class ReadThru : IReadThruProvider
    {
        private SqlConnection _connection;
        private readonly object _dbLock = new object();

        public void Init(IDictionary parameters, string cacheId)
        {
            try
            {
                string connString = GetConnectionString(parameters);
                if (!string.IsNullOrEmpty(connString))
                {
                    _connection = new SqlConnection(connString);
                    _connection.Open();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read-Thru Provider Init Failed: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        // *** UPDATED to fetch new fields ***
        private object LoadFromDataSource(string key)
        {
            if (!TryParseProductKey(key, out int productId)) return null;

            ProductProvider product = null;
            string query = "SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products WHERE ProductID = @Id";

            // Best practice to lock even for a single read to ensure thread safety
            // in the context of the provider which can be called by multiple threads.
            
                try
                {
                    // Use a new command object for each call
                    using (var command = new SqlCommand(query, _connection))
                    {
                        command.Parameters.Add("@Id", SqlDbType.Int).Value = productId;
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                product = new ProductProvider
                                {
                                    Id = Convert.ToInt32(reader["ProductID"]),
                                    Name = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["UnitPrice"]),
                                    CategoryId = Convert.ToInt32(reader["CategoryID"])
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Read-Thru LoadFromDataSource Failed for key '{key}': {ex.Message}");
                }
            

            return product;
        }
        // This part uses LoadFromDataSource, so no changes needed here
        public ProviderCacheItem LoadFromSource(string key)
        {
            object value = LoadFromDataSource(key);
            return value != null ? new ProviderCacheItem(value) : null;
        }

        // The rest of the class (bulk load, helpers) is below for completeness
        public IDictionary<string, ProviderCacheItem> LoadFromSource(ICollection<string> keys)
        {
            var result = new Dictionary<string, ProviderCacheItem>();
            var productIds = new List<int>();
            var keyMap = new Dictionary<int, string>();

            foreach (var key in keys)
            {
                if (TryParseProductKey(key, out int id))
                {
                    productIds.Add(id);
                    if (!keyMap.ContainsKey(id)) keyMap.Add(id, key);
                }
            }
            if (productIds.Count == 0) return result;

            // *** UPDATED QUERY for bulk load ***
            var queryBuilder = new StringBuilder("SELECT ProductID, ProductName, UnitPrice, CategoryID FROM Products WHERE ProductID IN (");
            // ... (rest of the bulk load logic is the same, but it will read the new object correctly below)
            var sqlParams = new List<SqlParameter>();
            for (int i = 0; i < productIds.Count; i++)
            {
                string paramName = $"@Id{i}";
                queryBuilder.Append(paramName + (i < productIds.Count - 1 ? "," : ""));
                sqlParams.Add(new SqlParameter(paramName, productIds[i]));
            }
            queryBuilder.Append(")");

            try
            {
                
                    using (var command = new SqlCommand(queryBuilder.ToString(), _connection))
                    {
                        command.Parameters.AddRange(sqlParams.ToArray());
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var product = new ProductProvider
                                {
                                    // *** UPDATED MAPPING for bulk load ***
                                    Id = Convert.ToInt32(reader["ProductID"]),
                                    Name = reader["ProductName"].ToString(),
                                    Price = Convert.ToDecimal(reader["UnitPrice"]),
                                    CategoryId = Convert.ToInt32(reader["CategoryID"])
                                };
                                string originalKey = keyMap[product.Id];
                                result[originalKey] = new ProviderCacheItem(product);
                            }
                        }
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Read-Thru (Bulk) operation failed: {ex.Message}");
            }
            return result;
        }
       
        private string GetConnectionString(IDictionary parameters)
        {
            if (parameters == null) return string.Empty;
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = parameters["server"] as string,
                InitialCatalog = parameters["database"] as string
            };
            string username = parameters["username"] as string;
            if (string.IsNullOrEmpty(username))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = username;
                builder.Password = parameters["password"] as string;
            }
            return builder.ConnectionString;
        }

        private bool TryParseProductKey(string key, out int id)
        {
            id = 0;
            if (string.IsNullOrEmpty(key)) return false;
            string[] parts = key.Split(':');
            return parts.Length == 2 && int.TryParse(parts[1], out id);
        }

        public ProviderDataTypeItem<IEnumerable> LoadDataTypeFromSource(string key, DistributedDataType dataType)
        {
            return null; // Not implemented for this demo
        }
    }
}