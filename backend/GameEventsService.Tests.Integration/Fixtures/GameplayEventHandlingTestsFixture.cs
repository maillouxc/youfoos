using System.Threading;
using Autofac.Extensions.DependencyInjection;
using DataAccess;
using DataAccess.Repositories;
using DataAccess.SharedTestUtils.TestData;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Fakes;

namespace GameEventsService.Tests.Integration.Fixtures
{
    public class GameplayEventHandlingTestsFixture
    {
        public readonly IModel GameEventsModel;
        public readonly IGamesRepository GamesRepository;
        public readonly RabbitServer FakeRabbitServer;
        public readonly RabbitMqConsumer GameEventsRabbitMqConsumer;
        public readonly AutoResetEvent MsgHandledEvent;

        private readonly AutofacServiceProvider _serviceProvider;

        public GameplayEventHandlingTestsFixture()
        {
            // Prepare the needed dependencies
            FakeRabbitServer = new RabbitServer();
            var fakeRabbitConnectionFactory = new FakeConnectionFactory(FakeRabbitServer);
            _serviceProvider = TestingDIConfig.GetTestingDIServiceProvider();
            GameEventsRabbitMqConsumer = _serviceProvider.GetService<RabbitMqConsumer>();
            GameEventsRabbitMqConsumer.InitConnection(fakeRabbitConnectionFactory);
            GameEventsModel = fakeRabbitConnectionFactory.Connection.CreateModel();
            GamesRepository = _serviceProvider.GetService<IGamesRepository>();

            ConfigureQueueBinding(FakeRabbitServer, "", "GameEvents");

            var inMemoryMongoContext = _serviceProvider.GetService<IMongoContext>();

            // Load some test users
            TestUsers.InsertIntoDatabase(inMemoryMongoContext, TestUsers.GetAllTestUsers())
                .GetAwaiter().GetResult();

            // Prepare a reset event to wait for on tests where we handle a message
            MsgHandledEvent = new AutoResetEvent(false);
            GameEventsRabbitMqConsumer.MessageHandledEvent += () => { MsgHandledEvent.Set(); };
            GameEventsRabbitMqConsumer.BeginConsuming();
        }

        private static void ConfigureQueueBinding(RabbitServer rabbitServer, string exchangeName, string queueName)
        {
            var connectionFactory = new FakeConnectionFactory(rabbitServer);
            using (var connection = connectionFactory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.ExchangeDeclare(exchangeName, ExchangeType.Direct, true, false, null);
                channel.QueueBind(queueName, exchangeName, queueName, null);
            }
        }
    }
}
