using System;
using Newtonsoft.Json;

namespace YouFoos.GameEventsService.Messages
{
    public class GoalUndoMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("game_guid")]
        public string GameGuid { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
