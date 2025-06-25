using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.DatasourceProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class WriteThru : IWriteThruProvider
    {   
        private SqlConnection _connection;

        // Perform tasks like allocating resources or acquiring connections
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
                // Handle exception
            }
        }

        // Perform tasks associated with freeing, releasing, or resetting resources.
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Close();
            }
        }

        //Responsible for write operations on data source
        public OperationResult WriteToDataSource(WriteOperation operation)
        {
            ProviderCacheItem cacheItem = operation.ProviderItem;
            Product product = cacheItem.GetValue<Product>();

            switch (operation.OperationType)
            {
                case WriteOperationType.Add:
                    // Insert logic for any Add operation
                    break;
                case WriteOperationType.Delete:
                    // Insert logic for any Delete operation
                    break;
                case WriteOperationType.Update:
                    // Insert logic for any Update operation
                    break;
            }
            // Write Thru operation status can be set according to the result.
            return new OperationResult(operation, OperationResult.Status.Success);
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<WriteOperation> operations)
        {
            var operationResult = new List<OperationResult>();
            foreach (WriteOperation operation in operations)
            {
                ProviderCacheItem cacheItem = operation.ProviderItem;
                Product product = cacheItem.GetValue<Product>();

                switch (operation.OperationType)
                {
                    case WriteOperationType.Add:
                        // Insert logic for any Add operation
                        break;
                    case WriteOperationType.Delete:
                        // Insert logic for any Delete operation
                        break;
                    case WriteOperationType.Update:
                        // Insert logic for any Update operation
                        break;
                }
                // Write Thru operation status can be set according to the result
                operationResult.Add(new OperationResult(operation, OperationResult.Status.Success));
            }
            return operationResult;
        }

        public ICollection<OperationResult> WriteToDataSource(ICollection<DataTypeWriteOperation> operations)
        {
            var operationResult = new List<OperationResult>();
            foreach (DataTypeWriteOperation operation in operations)
            {
                var list = new List<Product>();
                ProviderDataTypeItem<object> cacheItem = operation.ProviderItem;
                Product product = (Product)cacheItem.Data;

                switch (operation.OperationType)
                {
                    case DatastructureOperationType.CreateDataType:
                        // Insert logic for creating a new List
                        IList myList = new List<Product>();
                        myList.Add(product.Id);
                        break;
                    case DatastructureOperationType.AddToDataType:
                        // Insert logic for any Add operation
                        list.Add(product);
                        break;
                    case DatastructureOperationType.DeleteFromDataType:
                        // Insert logic for any Remove operation
                        list.Remove(product);
                        break;
                    case DatastructureOperationType.UpdateDataType:
                        // Insert logic for any Update operation
                        list.Insert(0, product);
                        break;
                }
                // Write Thru operation status can be set according to the result.
                operationResult.Add(new OperationResult(operation, OperationResult.Status.Success));
            }
            return operationResult;
        }


        // Parameters specified in Manager are passed to this method
        // These parameters make the connection string
        private string GetConnectionString(IDictionary parameters)
        {
            string connectionString = string.Empty;
            string server = parameters["server"] as string, database = parameters["database"] as string;
            string userName = parameters["username"] as string, password = parameters["password"] as string;
            try
            {
                connectionString = string.IsNullOrEmpty(server) ? "" : "Server=" + server + ";";
                connectionString = string.IsNullOrEmpty(database) ? "" : "Database=" + database + ";";
                connectionString += "User ID=";
                connectionString += string.IsNullOrEmpty(userName) ? "" : userName;
                connectionString += ";";
                connectionString += "Password=";
                connectionString += string.IsNullOrEmpty(password) ? "" : password;
                connectionString += ";";
            }
            catch (Exception exp)
            {
                // Handle exception
            }
            return connectionString;
        }
    }


}
