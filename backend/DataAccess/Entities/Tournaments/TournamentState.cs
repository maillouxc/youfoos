#pragma warning disable 1591

namespace YouFoos.DataAccess.Entities.Tournaments
{
    public enum TournamentState
    {
        Registration = 0,
        Seeding = 1,
        WaitingForStartTime = 2,
        InProgress = 3,
        Completed = 4,
        ErrorNotEnoughPlayers = 5,
        ErrorUnfinishedPastDeadline = 6
    }
}
