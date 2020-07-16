using LoggingManagement;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using MT.OnlineRestaurant.BusinessEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.BusinessLayer.ServiceBus
{
    public class ServiceBusHelper : IServiceBusHelper
    {
        private const string _serviceBusConnectionString = "Endpoint=sb://onlinerestaurant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=l9b+oqEFTQQIBJPFMHwqkcAoGJMn93ZcZDPdopPraRw=";
        
        private const string _requestTopicName = "isiteminstock301";
        //private const string _requestSubscriptionName = "IsItemInStockSubs301";

        private const string _responseTopicName = "isiteminstockresponse301";
        private const string _responseSubscriptionName = "IsItemInStockResponseSubs301";
        
        private static ITopicClient _topicClient;
        private static ISubscriptionClient _subscriptionClient;
        private readonly ILogService _logService;
        public static CartItemsEntity Response { get; set; }

        public ServiceBusHelper()
        {
            _topicClient = new TopicClient(_serviceBusConnectionString, _requestTopicName);
            _subscriptionClient = new SubscriptionClient(_serviceBusConnectionString, _responseTopicName, _responseSubscriptionName);
        }

        //public async Task SendMessagesAsync(List<string> messages)
        public async Task SendMessagesAsync(Dictionary<string, string> messages)
        {
            try
            {
                //_topicClient = new TopicClient(_serviceBusConnectionString, _requestTopicName);
                
                var message = JsonConvert.SerializeObject(messages);
                var messageEncoded = new Message(Encoding.UTF8.GetBytes(message));
                await _topicClient.SendAsync(messageEncoded);

                //await _topicClient.CloseAsync();
            }
            catch (Exception ex)
            {
                _logService.LogException(ex);
            }
        }

        public CartItemsEntity RecieveDataFromServiceBus()
        {
            //_subscriptionClient = new SubscriptionClient(_serviceBusConnectionString, _responseTopicName, _responseSubscriptionName);
            RegisterOnMessageHandlerAndReceiveMessages();
            return Response;
        }

        public static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var data = Encoding.UTF8.GetString(message.Body);
            Response = JsonConvert.DeserializeObject<CartItemsEntity>(data);
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

    }
}
