using System;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.Api.Dtos
{
    /// <summary>
    /// Represents a request to create a new tournament.
    /// </summary>
    public class CreateTournamentRequest
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int PlayerCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public GameType GameType { get; set; }
    }
}
