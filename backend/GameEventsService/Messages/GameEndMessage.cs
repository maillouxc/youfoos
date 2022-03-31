using System;
using Newtonsoft.Json;

namespace YouFoos.GameEventsService.Messages
{
    public class GameEndMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("game_guid")]
        public string GameGuid { get; set; }
        
        [JsonProperty("gold_score")]
        public int FinalGoldScore { get; set; }
        
        [JsonProperty("black_score")]
        public int FinalBlackScore { get; set; }
        
        [JsonProperty("final_duration_secs")]
        public long FinalDurationSeconds { get; set; }
        
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
