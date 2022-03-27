using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;

namespace YouFoos.DataAccess.Repositories
{
    /// <summary>
    /// Repository class for working with data for foosball in the database.
    /// </summary>
    public interface IGamesRepository
    {
        /// <summary>
        /// Inserts the given game into the database.
        /// 
        /// This method is also where the gameNumber field gets assigned a value.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the game already exists.</exception>
        Task InsertGame(Game game);

        /// <summary>
        /// Adds a given goal to the list of goals for a game, and adjusts the game score as appropriate.
        /// </summary>
        Task InsertGoal(Goal goal);

        /// <summary>
        /// Returns the game with the given ID, if it exists, or null if it does not.
        /// </summary>
        Task<Game> GetGameById(Guid id);

        /// <summary>
        /// Returns the currently ongoing game, if it exists.
        /// </summary>
        Task<Game> GetCurrentGame();

        /// <summary>
        /// Returns all games in chronological order. This method is only used for data remediation at the moment.
        /// </summary>
        Task<List<Game>> GetAllGamesChronological();

        /// <summary>
        /// Returns the number of games that are in the database.
        /// </summary>
        Task<long> CountGames();

        /// <summary>
        /// Returns the total number of goals that have been scored across all games.
        /// </summary>
        Task<int> CountGoalsScored();

        /// <summary>
        /// Returns the total amount of time played across all games, in seconds.
        /// </summary>
        Task<long> CountTimePlayedSecs();

        /// <summary>
        /// Returns the total number of wins by the given team.
        /// </summary>
        Task<int> CountTeamWins(TeamColor team);

        /// <summary>
        /// Returns a list of the n most recent games played by anyone.
        /// </summary>
        Task<List<Game>> GetListOfRecentGames(int numberOfGames);

        /// <summary>
        /// Returns a list of the n most recent games played by a user with the given id.
        ///
        /// If a cutoff date is specified, only games before that cutoff date are returned.
        /// </summary>
        Task<List<Game>> GetListOfRecentGamesByUserId(string id, DateTime cutoff, int numberOfGames = int.MaxValue);

        /// <summary>
        /// Given a game ID and a timestamp, undoes the previous goal from the game.
        ///
        /// If the game has no goals, returns without doing anything.
        /// </summary>
        Task UndoGoal(Guid gameGuid, DateTime timestamp);

        /// <summary>
        /// Sets the inProgress flag on the game with the given ID to false, and assigns the game an end time.
        /// </summary>
        /// <param name="gameGuid">The ID of the game in question.</param>
        /// <param name="endTimeUtc">The end time to assign to the game, in UTC.</param>
        Task EndGame(Guid gameGuid, DateTime endTimeUtc);

        /// <summary>
        /// Deletes the game with the given ID from the database, if it exists.
        /// </summary>
        Task DeleteGameByIdAsync(Guid gameGuid);
    }
}
