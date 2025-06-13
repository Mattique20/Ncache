using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Alachisoft.NCache.Client;
using System.Configuration;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;

namespace NcacheDemo
{
    class CRUDKeys
    {
        public void run(string CacheName, ICache _cache)
        {
            Console.WriteLine("NCache Bulk Operations Demo");

            try
            {
                _cache = CacheManager.GetCache(CacheName);

                if (_cache != null)
                {

                    // --- Part 1: Bulk Insert using a Loop ---

                    // Best Practice: Make the number of items configurable.
                    int numberOfProducts = 100;
                    Console.WriteLine($"Preparing to insert {numberOfProducts} products in bulk...");

                    // Create a dictionary to hold all the items for the single bulk operation.
                    var productItems = new Dictionary<string, CacheItem>();

                    // Loop to generate products and add them to the dictionary.
                    for (int i = 1; i <= numberOfProducts; i++)
                    {
                        // 1. Create a new product instance with unique data for each iteration.
                        var product = new Product
                        {
                            Id = 200 + i, // Start ID from 201 to make them unique
                            Name = $"Bulk Product #{i}",
                            Price = Math.Round(19.99 + (i * 2.5), 2) // Generate some varying price
                        };

                        // 2. Create the cache key using a consistent pattern.
                        string cacheKey = $"product:{product.Id}";

                        // 3. Add the key and a new CacheItem (wrapping the product) to the dictionary.
                        //    This dictionary is built in memory first.
                        productItems.Add(cacheKey, new CacheItem(product));
                    }

                    // 4. Perform the single, efficient bulk insert operation.
                    //    This sends all 100 items to the cache cluster in one network call.
                    _cache.InsertBulk(productItems);
                    Console.WriteLine($"Bulk insert operation completed for {productItems.Count} items.");
                    Console.WriteLine(new string('-', 40));


                    // --- Part 2: Bulk Get using a Loop ---

                    Console.WriteLine("Preparing to retrieve all products using GetBulk...");

                    // Create a list of all the keys we want to fetch.
                    var productKeysToFetch = new List<string>();

                    // Loop to generate the exact same keys we used for insertion.
                    // The key generation logic MUST match the insertion logic.
                    for (int i = 1; i <= numberOfProducts; i++)
                    {
                        productKeysToFetch.Add($"product:{200 + i}");
                    }

                    // Perform the single, efficient bulk get operation.
                    // This sends a request for all 100 keys in one network call.
                    IDictionary<string, Product> retrievedProducts = _cache.GetBulk<Product>(productKeysToFetch);

                    // --- Part 3: Process the Results ---

                    Console.WriteLine($"Successfully retrieved {retrievedProducts.Count} products from the cache.");

                    if (retrievedProducts.Count > 0)
                    {
                        Console.WriteLine("Retrieved Products:");
                        foreach (var kvp in retrievedProducts)
                        {
                            // The kvp.Value is already deserialized into a Product object.
                            Console.WriteLine($"  -> Key: {kvp.Key}, Value: {kvp.Value}");
                        }
                    }

                    // Best Practice: Check if you got back everything you asked for.
                    if (retrievedProducts.Count < numberOfProducts)
                    {
                        Console.WriteLine($"\nWARNING: {numberOfProducts - retrievedProducts.Count} items were not found. They might have been evicted or not inserted correctly.");
                    }


                    // Bulk Remove
                    //_cache.RemoveBulk(productKeysToFetch);
                    Console.WriteLine("Bulk removed products.");

                    // Confirm removal
                    var afterRemoval = _cache.GetBulk<Product>(productKeysToFetch);
                    foreach (var key in productKeysToFetch)
                    {
                        if (!afterRemoval.ContainsKey(key) || afterRemoval[key] == null)
                            Console.WriteLine($"Confirmed: '{key}' was removed.");
                        else
                            Console.WriteLine($"Warning: '{key}' still exists in cache.");
                    }

                    // Expiration using bulk insert
                    var tempItems = new Dictionary<string, CacheItem>
                    {
                        {
                            "temp:1",
                            new CacheItem("This will expire in 10s")
                            {
                                Expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromSeconds(15))
                            }
                        },
                        {
                            "temp:2",
                            new CacheItem("This will also expire")
                            {
                                Expiration = new Expiration(ExpirationType.Absolute, TimeSpan.FromSeconds(10))
                            }
                        }
                    };

                    /*//  _cache.InsertBulk(tempItems);
                      Console.WriteLine("Inserted temp items with 10-second expiration. Waiting 12 seconds...");
                      System.Threading.Thread.Sleep(12000);

                      var expiredCheck = _cache.GetBulk<CacheItem>(new string[] { "temp:1", "temp:2" });

                      foreach (var kvp in expiredCheck)
                      {
                          if (kvp.Value is CacheItem item)
                          {
                              Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value}");
                          }
                          else
                          {
                              Console.WriteLine($"Key: {kvp.Key} has expired or is not a CacheItem.");
                          }
                      }*/
                }
                else
                {
                    Console.WriteLine("Failed to connect to cache.");
                }
            }
            catch (OperationFailedException ex)
            {
                Console.WriteLine($"NCache Operation Failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                //_cache.Clear();
                //  _cache?.Dispose();
                Console.WriteLine("Cache cleared. Press any key to exit...");
            }

            Console.ReadKey();
        }
    }
}

