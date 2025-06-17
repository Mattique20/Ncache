using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Messaging;


namespace NcacheDemo
{
    class PubSub
    {
        private void OnFailureMessageReceived(object sender, MessageFailedEventArgs args)
        {

            Console.WriteLine("\n[CALLBACK TRIGGERED] ==> Message Delivery Failed!");
            Console.WriteLine($"   L-- Topic: '{args.TopicName}'");
            Console.WriteLine($"   L-- Reason for Failure: {args.MessageFailureReason}");
        }

        private void TopicDeleted(object sender, TopicDeleteEventArgs args)
        {
            // The 'sender' is the ITopic instance.
            // The 'args' object contains details about the deletion event.
            Console.WriteLine("\n[CALLBACK TRIGGERED] ==> The Topic Was Deleted!");
            Console.WriteLine($"   L-- Deleted Topic Name: '{args.TopicName}'");
        }
        
        public void run(ICache _cache, string topicName)
        {
            ITopic topic = _cache.MessagingService.CreateTopic(topicName, Alachisoft.NCache.Runtime.Messaging.TopicPriority.High);


            ITopic orderTopic = _cache.MessagingService.GetTopic(topicName);
            
            if (orderTopic != null)
            {
                Console.WriteLine($"Successfully created and got {orderTopic.Name}");
            }
            else
            {
                Console.WriteLine($"Notopic Exists");
            }




            var product = new Product { Id = 101, Name = "Laptop", Price = 1200 };
            var orderMessage = new Message(product);
            orderMessage.ExpirationTime = TimeSpan.FromSeconds(5000);
            orderTopic.Publish(orderMessage, DeliveryOption.All, true);
            
            // callbacks for notification
           // orderTopic.MessageDeliveryFailure += OnFailureMessageReceived;
           // orderTopic.OnTopicDeleted = TopicDeleted;



            _cache.MessagingService.DeleteTopicAsync(topicName);

            Thread.Sleep(2000);
            ITopic orderTopic2 = _cache.MessagingService.GetTopic(topicName);

            if (orderTopic2 != null)
            {
                Console.WriteLine($"Successfully created and got {orderTopic2.Name}");
            }
            else
            {
                Console.WriteLine($"No topic Exists");
            }
        }
    }
}
