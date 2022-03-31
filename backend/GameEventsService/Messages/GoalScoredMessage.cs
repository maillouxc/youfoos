using System;
using Newtonsoft.Json;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.GameEventsService.Messages
{
    public class GoalScoredMessage
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("game_guid")] 
        public string GameGuid { get; set; }
        
        [JsonProperty("scoring_player_rfid")] 
        public string ScoringPlayerRfid { get; set; }
       
        [JsonProperty("relative_timestamp")] 
        public long RelativeTimestamp { get; set; }
        
        [JsonProperty("team_scored_against")] 
        public TeamColor TeamScoredAgainst { get; set; }
        
        [JsonProperty("timestamp")] 
        public DateTime Timestamp { get; set; }
    }
}
