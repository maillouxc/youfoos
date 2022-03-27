using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.GameEventsService.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IGameToTournamentResolverService"/>.
    /// </summary>
    public class GameToTournamentResolverService : IGameToTournamentResolverService
    {
        private readonly ITournamentsRepository _tournamentsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameToTournamentResolverService(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
        }

        public async Task<Guid?> CheckIsTournamentGame(Game game)
        {
            Tournament currentTournament = await _tournamentsRepository.GetCurrentTournament();

            if (currentTournament == null) return null;

            if (currentTournament.GameType == GameType.Singles)
            {
                foreach (TournamentMatchup matchup in currentTournament.GetAllUnplayedMatchups())
                {
                    if (matchup.GoldUser1Id == game.GoldOffenseUserId && matchup.BlackUser1Id == game.BlackOffenseUserId)
                    {
                        game.TournamentId = currentTournament.Id;
                        game.TournamentMatchupId = matchup.Id;
                    }
                }

                return null;
            }
            else // GameType is doubles
            {
                var team1 = (user1: game.GoldOffenseUserId, user2: game.GoldDefenseUserId);
                var team2 = (user1: game.BlackOffenseUserId, user2: game.BlackDefenseUserId);

                foreach (TournamentMatchup matchup in currentTournament.GetAllUnplayedMatchups())
                {
                    bool hasTeam1 = matchup.HasTeam(team1.user1, team1.user2);
                    bool hasTeam2 = matchup.HasTeam(team2.user1, team2.user2);

                    if (hasTeam1 && hasTeam2)
                    {
                        game.TournamentId = currentTournament.Id;
                        game.TournamentMatchupId = matchup.Id;
                    }
                }

                return null;
            }
        }
    }
}
