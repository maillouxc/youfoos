using System;

namespace YouFoos.GameEventsService.Messages
{
    public class PostGameStatsEvent
    {
        public PostGameStatsEvent(Guid gameGuid)
        {
            GameGuid = gameGuid;
            Timestamp = DateTime.UtcNow;
        }

        public string Type { get; set; } = "PostGameStatsEvent";

        public Guid GameGuid { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}
