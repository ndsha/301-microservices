using Microsoft.Azure.Amqp;
using Microsoft.Azure.ServiceBus;
using MT.OnlineRestaurant.BusinessEntities;
using MT.OnlineRestaurant.DataLayer.Repository;
using Newtonsoft.Json;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MT.OnlineRestaurant.BusinessLayer.ServiceBus
{
    public class ServiceBusHelper : IServiceBusHelper
    {
        private const string _serviceBusConnectionString = "Endpoint=sb://onlinerestaurant.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=l9b+oqEFTQQIBJPFMHwqkcAoGJMn93ZcZDPdopPraRw=";
        private const string _responseTopicName = "isiteminstockresponse301";
        //private const string _responseSubscriptionName = "IsItemInStockResponseSubs301";
        private const string _requestTopicName = "isiteminstock301";
        private const string _requestSubscriptionName = "IsItemInStockSubs301";
        private static ITopicClient _topicClient;
        private static ISubscriptionClient _subscriptionClient;
        public static Dictionary<string, string> Response { get; set; }
        // private readonly ILogService _logService;
        private static ISearchRepository _searchRepository;
        public ServiceBusHelper(ISearchRepository searchRepository)
        {
            _subscriptionClient = new SubscriptionClient(_serviceBusConnectionString, _requestTopicName, _requestSubscriptionName);
            _topicClient = new TopicClient(_serviceBusConnectionString, _responseTopicName);
            _searchRepository = searchRepository;
        }

        public static async Task SendMessagesAsync(CartItemsEntity response)
        {
            try
            {
                //_topicClient = new TopicClient(_serviceBusConnectionString, _responseTopicName);

                //var message = JsonConvert.SerializeObject(data);
                //var messageEncoded = new Message(Encoding.UTF8.GetBytes(message));
                //await _topicClient.SendAsync(messageEncoded);

                //await _topicClient.CloseAsync();

                
                var data = JsonConvert.SerializeObject(response);
                var messageEncoded = new Message(Encoding.UTF8.GetBytes(data));
                await _topicClient.SendAsync(messageEncoded);
            }
            catch (Exception ex)
            {
                //_logService.LogException(ex);
            }
        }

        public void RecieveDataFromServiceBus()
        {
            RegisterOnMessageHandlerAndReceiveMessages();
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
            var messageReceived = Encoding.UTF8.GetString(message.Body);
            Response = JsonConvert.DeserializeObject<Dictionary<string, string>>(messageReceived);
            await _subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);

            if(Response != null)
            {
                Response.TryGetValue("RestaurantId", out string restaurantId);
                Response.TryGetValue("MenuId", out string menuId);
                Response.TryGetValue("Quantity", out string quantity);
                Response.TryGetValue("Price", out string price);
                var stockDetails = _searchRepository.ItemInStock(Convert.ToInt32(restaurantId), Convert.ToInt32(menuId));
                var priceDetails = _searchRepository.ItemPriceChaned(Convert.ToInt32(restaurantId), Convert.ToInt32(menuId));
                //var response = new CartItemsEntity();
                CartItemsEntity response;
                if(stockDetails == null || priceDetails == null)
                {
                    response = new CartItemsEntity()
                    {
                        RestaurantId = Convert.ToInt32(restaurantId),
                        ItemId = Convert.ToInt32(menuId),
                        Message = "Please enter valid RestaurantId and MenuId"
                    };
                }
                else
                {
                    response = new CartItemsEntity()
                    {
                        RestaurantId = Convert.ToInt32(restaurantId),
                        ItemId = Convert.ToInt32(menuId),
                        IsItemAvailable = Convert.ToInt32(quantity) <= stockDetails.quantity,
                        IsItemPriceChanged = Convert.ToDecimal(price) != priceDetails.Price,
                        ItemOldPrice = Convert.ToDecimal(price),
                        ItemNewPrice = priceDetails.Price
                    };
                }
                await SendMessagesAsync(response);
            }
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

    }
}
