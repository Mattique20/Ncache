using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;
using NcacheDemo; // Assuming your Product class is here
using System;
using System.Threading;

namespace NCacheDemoApp
{
    public static class OptimisticLocker
    {
        public static void ApplyDiscountOptimistically(ICache cache, string key, double discountPercentage, int maxRetries = 3)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    // --- 1. READ ITEM AND ITS VERSION ---
                    // GetCacheItem returns the object AND metadata, including the version.
                    CacheItem cachedItem = cache.GetCacheItem(key);
                    if (cachedItem == null)
                    {
                        Console.WriteLine("OPTIMISTIC: Item not found in cache.");
                        return;
                    }

                    Product product = cachedItem.GetValue<Product>();
                    Console.WriteLine($"OPTIMISTIC (Attempt #{i + 1}): Read item -> {product} with Version='{cachedItem.Version.Version}'");

                    // --- 2. MODIFY THE ITEM LOCALLY ---
                    double newPrice = Math.Round(product.Price * (1 - discountPercentage), 2);
                    Console.WriteLine($"OPTIMISTIC: Applying {discountPercentage:P0} discount. New price: {newPrice:C}.");
                    product.Price = newPrice;

                    // Create a new CacheItem to be inserted, preserving the original version
                    // This tells NCache: "Only update if the version in the cache matches this one"
                    var itemToUpdate = new CacheItem(product)
                    {
                        Version = cachedItem.Version
                    };

                    // --- 3. ATTEMPT TO UPDATE THE ITEM ---
                    // This operation will FAIL if the item's version on the server
                    // is different from 'cachedItem.Version'.
                    cache.Insert(key, itemToUpdate);

                    Console.WriteLine($"OPTIMISTIC: SUCCESS - Item updated in cache.");
                    return; // Success, exit the method.
                }
                catch (OperationFailedException ex)
                {
                    // This exception is thrown if the version check fails.
                    // This means another client updated the item after we read it.
                    Console.WriteLine($"OPTIMISTIC: Version conflict detected. Message: {ex.Message}");
                    if (i < maxRetries - 1)
                    {
                        Console.WriteLine("OPTIMISTIC: Retrying operation...");
                        Thread.Sleep(100); // Optional: wait a bit before retrying
                    }
                    else
                    {
                        Console.WriteLine($"OPTIMISTIC: FAILED - Could not update item after {maxRetries} retries.");
                    }
                }
            }
        }
    }
}