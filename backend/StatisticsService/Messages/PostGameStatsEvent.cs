using System;
using System.Diagnostics.CodeAnalysis;

namespace YouFoos.StatisticsService.Messages
{
    [ExcludeFromCodeCoverage]
    public class PostGameStatsEvent
    {
        public PostGameStatsEvent(Guid gameGuid, DateTime timestamp)
        {
            GameGuid = gameGuid;
            Timestamp = timestamp;
        }

        public string Type { get; set; } = "PostGameStatsEvent";
        public Guid GameGuid { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
