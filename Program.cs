// Program.cs
using System;
using System.Collections.Generic;
using System.Threading;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Exceptions;
using NcacheDemo;

namespace NCacheDemoApp
{
    class Program
    {
        //private static ICache _cache;
        private const string CacheName = "59PORClusteredCache";
        static void Main(string[] args)
        {

            ICache _cache = CacheManager.GetCache(CacheName);
            try
            {
                string topicName = "orderTopic";

                PubSub pub = new PubSub();
                pub.run(_cache, topicName);
               
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
                // _cache.Clear();
                //_cache?.Dispose();
                //Console.WriteLine("Cache cleared. Press any key to exit...");
                
            }

            Console.ReadKey();
        }
       
    }
}