using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.DatasourceProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Providers
{
    public class WriteThru : IWriteThruProvider
    {
        // We only store the connection STRING, not the shared connection object.
        private string _connectionString;

        /// <summary>
        /// This method is called once when the cache starts. Its only job is to get 
        /// the connection string and test it to ensure the provider can connect to the DB.
        /// </summary>
        public void Init(IDictionary parameters, string cacheId)
        {
            try
            {
               
                var builder = new SqlConnectionStringBuilder();
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

                if (string.IsNullOrEmpty(builder.DataSource) || string.IsNullOrEmpty(builder.InitialCatalog))
                {
                    throw new Exception("Required parameters 'server' and 'database' are missing.");
                }

                string username = GetParam("username");
                if (string.IsNullOrEmpty(username))
                {
                    builder.IntegratedSecurity = true;
                }
                else
                {
                    builder.UserID = username;
                    builder.Password = GetParam("password");
                }

                // Store the connection string for later use.
                _connectionString = builder.ConnectionString;

                // Test the connection once at startup. If this fails, the cache will not start.
                using (var tempConn = new SqlConnection(_connectionString))
                {
                    tempConn.Open();
                }
            }
            catch (Exception ex)
            {
                // This stops the cache from starting if the DB connection is bad and logs the real error.
                throw new Exception($"[WriteThru] Provider initialization failed. Check connection string and database access. Original Error: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// We no longer have a persistent connection to dispose of.
        /// </summary>
        public void Dispose()
        {
            // Nothing to do here.
        }

        /// <summary>
        /// This method is called for every write operation. It is now fully self-contained and thread-safe.
        /// </summary>
        public OperationResult WriteToDataSource(WriteOperation operation)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    ProductProvider product = operation.ProviderItem?.GetValue<ProductProvider>();

                    if (product == null && operation.OperationType != WriteOperationType.Delete)
                    {
                        throw new Exception("Product object for write operation is null.");
                    }

                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        switch (operation.OperationType)
                        {
                            case WriteOperationType.Add:
                                // This block is designed for auto-increment keys.
                                command.CommandText = "INSERT INTO Products (ProductName, UnitPrice, CategoryID) VALUES (@Name, @Price, @CategoryId); SELECT SCOPE_IDENTITY();";
                                command.Parameters.AddWithValue("@Name", product.Name);
                                command.Parameters.AddWithValue("@Price", product.Price);
                                command.Parameters.AddWithValue("@CategoryId", product.CategoryId);

                                object newId = command.ExecuteNonQuery();
                                if (newId != null && newId != DBNull.Value)
                                {
                                    product.Id = Convert.ToInt32(newId);
                                    var updatedCacheItem = new CacheItem(product);
                                    string newKey = $"ProductProvider:{product.Id}";

                                    // This special result tells NCache to update the key and value in the cache.
                                    var writeResult = new OperationResult(operation, OperationResult.Status.Success, "Updated with DB-generated ID");
                                   
                                    return writeResult;
                                }
                                else
                                {
                                    throw new Exception("Failed to retrieve new ID from database after insert.");
                                }

                            case WriteOperationType.Update:
                                // Update logic remains the same.
                                command.CommandText = "UPDATE Products SET ProductName = @Name, UnitPrice = @Price, CategoryID = @CategoryId WHERE ProductID = @Id";
                                command.Parameters.AddWithValue("@Name", product.Name);
                                command.Parameters.AddWithValue("@Price", product.Price);
                                command.Parameters.AddWithValue("@CategoryId", product.CategoryId);
                                command.Parameters.AddWithValue("@Id", product.Id);
                                command.ExecuteNonQuery();
                                break;

                            case WriteOperationType.Delete:
                                // Delete logic remains the same.
                                command.CommandText = "DELETE FROM Products WHERE ProductID = @Id";
                                command.Parameters.AddWithValue("@Id", Convert.ToInt32(operation.Key.Substring(operation.Key.LastIndexOf(':') + 1)));
                                command.ExecuteNonQuery();
                                break;
                        }
                    }
                }
                return new OperationResult(operation, OperationResult.Status.Success);
            }
            catch (Exception ex)
            {
                return new OperationResult(operation, OperationResult.Status.Failure, ex);
            }
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<WriteOperation> operations)
        {
            var operationResults = new List<OperationResult>();
            foreach (var op in operations)
            {
                operationResults.Add(WriteToDataSource(op));
            }
            return operationResults;
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<DataTypeWriteOperation> operations)
        {
            return new List<OperationResult>();
        }
    }
}
