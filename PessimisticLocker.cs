using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using NcacheDemo; // Assuming your Product class is here
using System;

namespace NCacheDemoApp
{
    public static class PessimisticLocker
    {
        public static void ApplyDiscountPessimistically(ICache cache, string key, double discountPercentage)
        {
            LockHandle lockHandle = new LockHandle();
            // The lock will be automatically released by the server if our client
            // crashes and does not unlock it within 30 seconds.
            TimeSpan lockTimeout = TimeSpan.FromSeconds(30);

            try
            {
                // --- 1. ACQUIRE THE EXCLUSIVE LOCK ---
                bool isLockAcquired = cache.Lock(key, lockTimeout, out lockHandle);
                
                if (isLockAcquired)
                {
                    Console.WriteLine($"PESSIMISTIC: Lock acquired on key '{key}'.");

                    // --- 2. GET & MODIFY THE LOCKED ITEM ---
                    var product = cache.Get<Product>(key);
                    if (product != null)
                    {
                        Console.WriteLine($"PESSIMISTIC: Read locked item -> {product}");

                        double newPrice = Math.Round(product.Price * (1 - discountPercentage), 2);
                        Console.WriteLine($"PESSIMISTIC: Applying {discountPercentage:P0} discount. New price: {newPrice:C}.");
                        product.Price = newPrice;

                        var cacheItem = new CacheItem(product);

                        // --- 3. UPDATE THE ITEM (Must provide the lock handle) ---
                        cache.Insert(key, cacheItem, lockHandle, true);

                        Console.WriteLine($"PESSIMISTIC: SUCCESS - Item updated in cache.");
                    }
                }
                else
                {
                    // This code runs if cache.Lock() returned false
                    Console.WriteLine("PESSIMISTIC: Could not acquire lock. Another process is updating this item.");
                }
            }
            finally
            {
                // --- 4. CRITICAL: RELEASE THE LOCK ---
                if (lockHandle != null && !string.IsNullOrEmpty(lockHandle.LockId))
                {
                    cache.Unlock(key, lockHandle);
                    Console.WriteLine($"PESSIMISTIC: Lock released for key '{key}'.");
                }
            }
        }
    }
}