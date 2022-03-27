using System.Diagnostics.CodeAnalysis;

namespace YouFoos.StatisticsService.Config
{
    [ExcludeFromCodeCoverage]
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
