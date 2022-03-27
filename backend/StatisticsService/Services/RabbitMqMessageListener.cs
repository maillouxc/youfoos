using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;
using YouFoos.StatisticsService.Config;
using YouFoos.StatisticsService.Messages;

namespace YouFoos.StatisticsService.Services
{
    public class RabbitMqMessageListener
    {
        private readonly RabbitMqSettings _rabbitSettings;

        private readonly IStatsCalculator _statsCalculator;
        private readonly IAccoladesCalculator _accoladesCalculator;
        private readonly IAchievementUnlockService _achievementUnlockService;
        private readonly ITournamentGameHandler _tournamentGameHandler;

        private IConnection Connection { get; set; }
        private IModel ConsumerChannel { get; set; }

        public event Action MessageHandledEvent;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RabbitMqMessageListener(IOptions<RabbitMqSettings> rabbitSettings, 
                                       IStatsCalculator statsCalculator,
                                       IAccoladesCalculator accoladesCalculator,
                                       IAchievementUnlockService achievementUnlockService,
                                       ITournamentGameHandler tournamentGameHandler)
        {
            _rabbitSettings = rabbitSettings.Value;
            _statsCalculator = statsCalculator;
            _accoladesCalculator = accoladesCalculator;
            _achievementUnlockService = achievementUnlockService;
            _tournamentGameHandler = tournamentGameHandler;
        }

        public void InitConnection(ConnectionFactory connectionFactory)
        {
            connectionFactory.Uri = new Uri(_rabbitSettings.ConnectionString);
            Connection = connectionFactory.CreateConnection();

            ConsumerChannel = Connection.CreateModel();
            ConsumerChannel.QueueDeclare(
                queue: (_rabbitSettings.QueueName),
                durable: true,     // Queue will persist after broker is restarted
                exclusive: false,  // Queue can be used by multiple connections
                autoDelete: false, // Queue is not deleted when the last consumer unsubscribes.
                arguments: null);

            Log.Logger.Information("RabbitMq connection initialized successfully.");
        }

        public void BeginConsuming()
        {
            var messageConsumer = new EventingBasicConsumer(ConsumerChannel);

            messageConsumer.Received += (model, eventArgs) =>
            {
                string message = Encoding.UTF8.GetString(eventArgs.Body.Span);
                HandleMessage(message);
                ConsumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                MessageHandledEvent?.Invoke();
            };

            ConsumerChannel.BasicConsume(queue: _rabbitSettings.QueueName,
                                         autoAck: false,
                                         consumer: messageConsumer);

            Log.Logger.Information("Now consuming RabbitMq messages in channel: {queueName}", _rabbitSettings.QueueName);
        }

        private async void HandleMessage(string message)
        {
            try
            {
                var msg = JsonConvert.DeserializeObject<PostGameStatsEvent>(message);
                await HandlePostGameStatsEventMessage(msg);
            }
            catch (Exception e)
            {
                Log.Logger.Error("Error calculating stats: {@e}", e);
            }
        }

        private async Task HandlePostGameStatsEventMessage(PostGameStatsEvent msg)
        {
            Log.Logger.Information("Processing PostGameStatsEvent");
            
            // TODO it's probably better to pass in the game itself here instead of reading it from the db repeatedly.
            await _statsCalculator.RecalculateStatsAfterGame(msg.GameGuid);
            await _accoladesCalculator.RecalculateAllAccolades();
            await _achievementUnlockService.UpdateAchievementStatusesPostGame(msg.GameGuid);
            await _tournamentGameHandler.HandleGameIfIsTournamentGame(msg.GameGuid);
        }
    }
}
