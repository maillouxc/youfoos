using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using YouFoos.GameEventsService.Config;
using YouFoos.GameEventsService.Messages;
using YouFoos.GameEventsService.Services;

namespace YouFoos.GameEventsService
{
    public class RabbitMqConsumer
    {
        private readonly RabbitMqSettings _rabbitSettings;

        private IConnection Connection { get; set; }

        // IModel is used in place of "Channel" in the C# RabbitMQ client
        private IModel ConsumerChannel { get; set; }
        private IModel ProducerChannel { get; set; }

        private readonly IGameplayMessageHandler _messageHandler;

        /// <summary>
        /// This event is called when the class finishes handling each message received.
        ///
        /// This can be useful in many cases, but it was primarily implemented for integration testing.
        /// </summary>
        public event Action MessageHandledEvent;
        
        public RabbitMqConsumer(IOptions<RabbitMqSettings> rabbitSettings, 
                                IGameplayMessageHandler messageHandler)
        {
            _rabbitSettings = rabbitSettings.Value;
            _messageHandler = messageHandler;
        }

        /// <summary>
        /// Initializes the connection to the rabbit queue, but does not begin consuming messages.
        /// </summary>
        public void InitConnection(ConnectionFactory connectionFactory)
        {
            connectionFactory.Uri = new Uri(_rabbitSettings.ConnectionString);
            Connection = connectionFactory.CreateConnection();
                       
            ConsumerChannel = Connection.CreateModel();
            ConsumerChannel.QueueDeclare( 
                queue: (_rabbitSettings.GamesQueueName),
                durable: true,     // Queue will persist after broker is restarted
                exclusive: false,  // Queue can be used by multiple connections
                autoDelete: false, // Queue is not deleted when the last consumer unsubscribes.
                arguments: null);

            ProducerChannel = Connection.CreateModel();
            ProducerChannel.QueueDeclare(
                queue: (_rabbitSettings.StatsQueueName),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            Log.Logger.Information("RabbitMq connection initialized successfully");
        }

        /// <summary>
        /// Starts consuming messages from the GameEvents RabbitMQ.
        /// </summary>
        public void BeginConsuming()
        {
            var messageConsumer = new EventingBasicConsumer(ConsumerChannel);
            
            messageConsumer.Received += (model, eventArgs) =>
            {
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);

                string message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                HandleMessageAsync(message).GetAwaiter().GetResult();

                // Send a message to the stats service to recalculate the stats when each game ends
                if (GameplayMessageHandler.GetMessageType(message) == "gameEnd")
                {
                    var jsonMsgObject = (JObject)JsonConvert.DeserializeObject(message);
                    var gameGuid = (Guid) jsonMsgObject["game_guid"];
                    SendPostGameStatsEventToStatsService(gameGuid);
                }
                
                MessageHandledEvent?.Invoke();
            };

            ConsumerChannel.BasicConsume(
                queue: _rabbitSettings.GamesQueueName,
                autoAck: false,
                consumer: messageConsumer);

            Log.Logger.Information("Now consuming RabbitMq messages in channel: {queueName}", _rabbitSettings.GamesQueueName);
        }

        /// <summary>
        /// Method to handle any messages sent to the queue.
        ///
        /// If an exception occurs, it will be caught and logged, to prevent crashing the whole
        /// service if an invalid message is sent, for instance.
        /// </summary> 
        private async Task HandleMessageAsync(string message)
        {
            try
            {
                await _messageHandler.HandleMessageAsync(message);
            }
            catch (Exception e)
            {
                Log.Logger.Error("Exception while handling message {@m}: {@e}", message, e);
            }
        }

        private void SendPostGameStatsEventToStatsService(Guid gameGuid)
        {
            var statsEventMsg = new PostGameStatsEvent(gameGuid);
            Log.Logger.Information("Sending stats event message: {@stats}", statsEventMsg);
            var statsEventMsgJson = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(statsEventMsg));
            ProducerChannel.BasicPublish("", _rabbitSettings.StatsQueueName, null, statsEventMsgJson);
        }
    }
}
