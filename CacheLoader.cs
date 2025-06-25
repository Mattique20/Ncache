using Alachisoft.NCache.Runtime.CacheLoader;
using Alachisoft.NCache.Runtime.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NcacheDemo;
using NcacheDemo.SampleData;

namespace NcacheDemo
{
    class CacheLoader : ICacheLoader
    {
        public void Init(IDictionary<string, string> parameters, string cacheName)
        {
            Console.WriteLine($"CacheLoader for cache '{cacheName}'");
        }
        public object LoadDatasetOnStartup(string dataSet)
        {
            if (string.IsNullOrEmpty(dataSet)) return null;

            Console.WriteLine($"LOADER: Loading dataset '{dataSet}' on startup...");
            var itemsToLoad = new List<ProviderCacheItem>();

            switch (dataSet.ToLower())
            {
                case "products":
                    var allProducts = MockDB.Products;
                    foreach (var product in allProducts)
                    {
                        var item = new ProviderCacheItem(product);
                        item.ResyncOptions = new ResyncOptions(true, dataSet);
                        itemsToLoad.Add(item);
                    }
                    break;

                case "suppliers":
                    var allSuppliers = MockDB.Suppliers;
                    foreach (var supplier in allSuppliers)
                    {
                        var item = new ProviderCacheItem(supplier);
                        item.ResyncOptions = new ResyncOptions(true, dataSet);
                        itemsToLoad.Add(item);
                    }
                    break;
            }

            Console.WriteLine($"LOADER: Returning {itemsToLoad.Count} items for NCache to load.");
            return itemsToLoad.ToArray();
        }
        public IDictionary<string, RefreshPreference> GetDatasetsToRefresh(IDictionary<string, object> userContexts)
        {
            var datasetsToRefresh = new Dictionary<string, RefreshPreference>();

            foreach (var dataSet in userContexts.Keys)
            {
                DateTime? lastCheckTime = userContexts[dataSet] as DateTime?;

                switch (dataSet.ToLower())
                {
                    case "products":
                        if (MockDB.Products.Any(p => p.LastModify > lastCheckTime))
                        {
                            Console.WriteLine("REFRESHER: Product dataset has changed. Scheduling for refresh.");
                            datasetsToRefresh.Add(dataSet, RefreshPreference.RefreshNow);
                        }
                        break;

                    case "suppliers":
                        if (MockDB.Suppliers.Any(s => s.LastModify > lastCheckTime))
                        {
                            Console.WriteLine("REFRESHER: Supplier dataset has changed. Scheduling for refresh.");
                            datasetsToRefresh.Add(dataSet, RefreshPreference.RefreshNow);
                        }
                        break;
                }
            }

            return datasetsToRefresh;
        }

        public object RefreshDataset(string dataSet, object userContext)
        {
            if (string.IsNullOrEmpty(dataSet)) return null;

            Console.WriteLine($"REFRESHER: Fetching updated items for dataset '{dataSet}'...");
            var itemsToRefresh = new List<ProviderCacheItem>();
            DateTime? lastCheckTime = userContext as DateTime?;

            switch (dataSet.ToLower())
            {
                case "products":
                    var updatedProducts = MockDB.Products.Where(p => p.LastModify > lastCheckTime);
                    foreach (var product in updatedProducts)
                    {
                        var item = new ProviderCacheItem(product);
                        // Re-apply ReSyncOptions so it gets checked again in the future.
                        item.ResyncOptions = new ResyncOptions(true, dataSet);
                        itemsToRefresh.Add(item);
                    }
                    break;

                case "suppliers":
                    var updatedSuppliers = MockDB.Suppliers.Where(s => s.LastModify > lastCheckTime);
                    foreach (var supplier in updatedSuppliers)
                    {
                        var item = new ProviderCacheItem(supplier);
                        item.ResyncOptions = new ResyncOptions(true, dataSet);
                        itemsToRefresh.Add(item);
                    }
                    break;
            }

            Console.WriteLine($"REFRESHER: Returning {itemsToRefresh.Count} updated items for NCache to refresh.");
            return itemsToRefresh.ToArray();
        }


        // The user context provider method is used to pass state (like the current time)
        // between the Refresher calls.
        public object GetUserContext(string dataset)
        {
            return DateTime.UtcNow;
        }

        public void Dispose()
        {
            // Nothing to dispose of in this mock implementation.
        }
    }
}
