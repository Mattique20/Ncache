using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;


namespace NcacheDemo
{
    class ManagementLevelEvents
    {
        #region Event Handler Methods
        public void OnCacheCleared()
        {
            Console.WriteLine("\n[EVENT RECEIVED] ==> The entire cache was cleared!");
        }
        public void OnCacheStopped(string cacheName)
        {
            Console.WriteLine($"\n[EVENT RECEIVED] ==> The cache '{cacheName}' has been stopped!");
            Console.WriteLine("   L-- This client is now disconnected.");
        }

        public void OnMemberJoined(NodeInfo nodeInfo)
        {
            Console.WriteLine("\n[EVENT RECEIVED] ==> A member has JOINED the cluster.");
            Console.WriteLine($"   L-- Node IP Address: {nodeInfo.IpAddress.Address}");
            Console.WriteLine($"   L-- Node Status: {(nodeInfo.Port)}");
        }

 
        public void OnMemberLeft(NodeInfo nodeInfo)
        {
            Console.WriteLine("\n[EVENT RECEIVED] ==> A member has LEFT the cluster.");
            Console.WriteLine($"   L-- Node IP Address: {nodeInfo.IpAddress.Address}");
            Console.WriteLine($"   L-- Node Status: {(nodeInfo.Port)}");
        }
        #endregion

        public void Run(string cacheName)
        {
            ICache _cache = CacheManager.GetCache(cacheName);
            try
            {
               
                Console.WriteLine($"Successfully connected to cache: {cacheName}");

                Console.WriteLine("\n--- Registering General and Cluster Event Handlers ---");
                _cache.NotificationService.CacheCleared += OnCacheCleared;
                _cache.NotificationService.CacheStopped += OnCacheStopped;
                _cache.NotificationService.MemberJoined += OnMemberJoined;
                _cache.NotificationService.MemberLeft += OnMemberLeft;

                Console.WriteLine("--> Event handlers registered. Application is now listening.");
                Console.WriteLine("----------------------------------------------------------\n");

                while (true)
                {
                    string input = Console.ReadLine();
                    if (input?.ToLower() == "q")
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                if (_cache != null)
                {
                    Console.WriteLine("\n--- Unregistering event handlers and disconnecting... ---");
                    _cache.NotificationService.CacheCleared -= OnCacheCleared;
                    _cache.NotificationService.CacheStopped -= OnCacheStopped;
                    _cache.NotificationService.MemberJoined -= OnMemberJoined;
                    _cache.NotificationService.MemberLeft -= OnMemberLeft;
                    _cache.Dispose();
                    Console.WriteLine("--> Disconnected successfully.");
                }
            }
        }
    }
}
