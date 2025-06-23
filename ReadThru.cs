using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.DatasourceProviders;
using NcacheDemo.SampleData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NcacheDemo
{
    class ReadThru : IReadThruProvider
    {

        public void Init(IDictionary parameters, string cacheId)
        {
            Console.WriteLine($"CacheLoader for cache '{cacheId}'");
        }

        public ProviderCacheItem LoadFromSource(string key)
        {
            object value = LoadFromDataSource(key);
            var cacheItem = new ProviderCacheItem(value);
            return cacheItem;
        }

        public IDictionary<string, ProviderCacheItem> LoadFromSource(ICollection<string> keys)
        {
            var dictionary = new Dictionary<string, ProviderCacheItem>();
            try
            {
                foreach (string key in keys)
                {
                    // LoadFromDataSource loads data from data source
                    dictionary.Add(key, new ProviderCacheItem(LoadFromDataSource(key)));
                }
                return dictionary;
            }
            catch (Exception exp)
            {
                // Handle exception
            }
            return dictionary;
        }
        public ProviderDataTypeItem<IEnumerable> LoadDataTypeFromSource(string key, DistributedDataType dataType)
        {
            IEnumerable value = null;
            ProviderDataTypeItem<IEnumerable> dataTypeItem = null;

            switch (dataType)
            {
                case DistributedDataType.List:
                    value = new List<object>()
                    {
                        LoadFromDataSource(key)
                    };
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(value);
                    break;
                case DistributedDataType.Dictionary:
                    value = new Dictionary<string, object>()
                    {
                        { key ,  LoadFromDataSource(key) }
                    };
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(value);
                    break;
                case DistributedDataType.Counter:
                    dataTypeItem = new ProviderDataTypeItem<IEnumerable>(1000);
                    break;
            }
            return dataTypeItem;
        }

        public void Dispose()
        {

        }

        private object LoadFromDataSource(string key)
        {
            object retrievedObject = null;
            if (string.IsNullOrEmpty(key)) return null;

            Console.WriteLine($"LOADER: Loading dataset '{key}' on startup...");
            var itemsToLoad = new List<ProviderCacheItem>();

            switch (key.ToLower())
            {
                case "products":
                    var allProducts = MockDB.Products;
                    foreach (var product in allProducts)
                    {
                        var item = new ProviderCacheItem(product);
                        item.ResyncOptions = new ResyncOptions(true, key);
                        itemsToLoad.Add(item);
                    }
                    break;

                case "suppliers":
                    var allSuppliers = MockDB.Suppliers;
                    foreach (var supplier in allSuppliers)
                    {
                        var item = new ProviderCacheItem(supplier);
                        item.ResyncOptions = new ResyncOptions(true, key);
                        itemsToLoad.Add(item);
                    }
                    break;
            }

            Console.WriteLine($"LOADER: Returning {itemsToLoad.Count} items for NCache to load.");
            return itemsToLoad.ToArray();

            return retrievedObject;
        }

        public void Run(ICache _cache)
        {

            var readThruOptions = new ReadThruOptions();
            readThruOptions.Mode = ReadMode.ReadThru;

            Product data = _cache.Get<Product>("product; #201", readThruOptions);
        }
    }
}
