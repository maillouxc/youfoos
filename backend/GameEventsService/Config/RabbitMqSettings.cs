namespace YouFoos.GameEventsService.Config
{
    public class RabbitMqSettings
    {
        public string ConnectionString { get; set; }

        public string GamesQueueName { get; set; }

        public string StatsQueueName { get; set; }
    }
}
