using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace YouFoos.GameEventsService.Messages
{
    [ExcludeFromCodeCoverage]
    public class GameStartMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("game_guid")]
        public string GameGuid { get; set; }
        
        [JsonProperty("game_type")]
        public string GameType { get; set; }
        
        [JsonProperty("gold_offense_rfid")]
        public string GoldOffenseRfid { get; set; }
       
        [JsonProperty("gold_defense_rfid")]
        public string GoldDefenseRfid { get; set; }
        
        [JsonProperty("black_offense_rfid")]
        public string BlackOffenseRfid { get; set; }
        
        [JsonProperty("black_defense_rfid")]
        public string BlackDefenseRfid { get; set; }
        
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
