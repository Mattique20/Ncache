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
            
            Console.WriteLine("\n[CALLBACK TRIGGERED] ==> The Topic Was Deleted!");
            Console.WriteLine($"   L-- Deleted Topic Name: '{args.TopicName}'");
        }
        private void MessageReceived(object sender, MessageEventArgs args)
        {
            
            Console.WriteLine($"\n[MESSAGE RECEIVED on Topic: '{args.TopicName}']");
            if (args.Message.Payload is Product receivedProduct)
            {
                Console.WriteLine($"   L-- Received Product Details: {receivedProduct.Name}");
                Console.WriteLine("       L-- Simulating order processing for this product...");
            }
            else
            {
                Console.WriteLine("   L-- Received a message, but it could not be recognized as a Product.");
            }
        }


            public void run(ICache _cache, string topicName)
            {

           
 
                  //  _cache.MessagingService.DeleteTopic(topicName);
                    ITopic topic = _cache.MessagingService.CreateTopic(topicName, Alachisoft.NCache.Runtime.Messaging.TopicPriority.High);


                    ITopic orderTopic = _cache.MessagingService.GetTopic("Hello World");

                    //-----------------------------------------------------------PUBLISHER-----------------------------------------------------//
                    //Publish one object
                    var product = new Product { Id = 101, Name = "Laptop", Price = 1200 };
                    var orderMessage = new Message(product);
                    orderMessage.ExpirationTime = TimeSpan.FromSeconds(5000);
                    topic.Publish(orderMessage, DeliveryOption.Any, true);

                    //-----------------------------------------------------------Publish Bulk
                    var messagesWithDeliveryOptions = new List<Tuple<Message, DeliveryOption>>();

                    // Message 1:
                    var product1 = new Product { Id = 101, Name = "Laptop", Price = 1200 };
                    var message1 = new Message(product1);
                    messagesWithDeliveryOptions.Add(Tuple.Create(message1, DeliveryOption.All));

                    // Message 2:
                    var product2 = new Product { Id = 102, Name = "Mouse", Price = 25 };
                    var message2 = new Message(product2);
                    messagesWithDeliveryOptions.Add(Tuple.Create(message2, DeliveryOption.Any));


                    // Message 1:
                    var product3 = new Product { Id = 103, Name = "Keyboard", Price = 1200 };
                    var message3 = new Message(product3);
                    messagesWithDeliveryOptions.Add(Tuple.Create(message3, DeliveryOption.All));

                    // Message 2:
                    var product4 = new Product { Id = 104, Name = "Stand", Price = 25 };
                    var message4 = new Message(product4);
                    messagesWithDeliveryOptions.Add(Tuple.Create(message4, DeliveryOption.Any));

                    IDictionary<Message, Exception> failedMessages = orderTopic.PublishBulk(messagesWithDeliveryOptions);


                    Console.WriteLine("Items published in Bulk");

                    //-----------------------------------------------------------SUBSCRIBER-----------------------------------------------------//


                    //-------subscription for receiving topic
                    ITopicSubscription subscription = topic.CreateSubscription(MessageReceived, DeliveryMode.Sync);
                    string subscriptionName = "Hello World";
                    ///IDurableTopicSubscription orderSubscriber = orderTopic.CreateDurableSubscription(subscriptionName, SubscriptionPolicy.Shared, MessageReceived);



                    //------Subscribing to message failure and on deletion
                    topic.MessageDeliveryFailure += OnFailureMessageReceived;
                    //topic.OnTopicDeleted = TopicDeleted;
                    Console.WriteLine("Subscription Created");


                         Thread.Sleep(500);
                       //_cache.MessagingService.DeleteTopic(topicName);

                     var newOrder = new Product { Id = 205, Name = "Gaming Headset", Price = 150 };
                     var orderMessage2 = new Message(newOrder); // The 'newOrder' object goes into the .Value property here.

                   topic.Publish(orderMessage2, DeliveryOption.All);

                    Console.WriteLine("--> Message has been published to the topic.");
                
            }

    }
}
