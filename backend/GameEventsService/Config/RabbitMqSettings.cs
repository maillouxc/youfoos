using System.Diagnostics.CodeAnalysis;

namespace YouFoos.GameEventsService.Config
{
    [ExcludeFromCodeCoverage]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string GamesQueueName { get; set; }
        public string StatsQueueName { get; set; }
    }
}
