using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Entities.Stats;
using YouFoos.DataAccess.Repositories;
using YouFoos.StatisticsService.StatCalculations;

namespace YouFoos.StatisticsService.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IStatsCalculator"/>.
    /// </summary>
    public class StatsCalculator : IStatsCalculator
    {
        private readonly ITrueskillCalculator _skillCalculator;

        private readonly IUsersRepository _usersRepository;
        private readonly IGamesRepository _gamesRepository;
        private readonly IStatsRepository _statsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatsCalculator(ITrueskillCalculator skillCalculator,
                               IUsersRepository usersRepository,
                               IGamesRepository gamesRepository,
                               IStatsRepository statsRepository)
        {
            _skillCalculator = skillCalculator;
            _usersRepository = usersRepository;
            _gamesRepository = gamesRepository;
            _statsRepository = statsRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsCalculator.RecalculateStatsAfterGame(Guid)"./>
        /// </summary>
        public async Task RecalculateStatsAfterGame(Guid gameId)
        {
            var game = await _gamesRepository.GetGameById(gameId);

            // Get all the players from the game (if it was a 1v1 these may be duplicate - its okay)
            var goUser = await _usersRepository.GetUserWithId(game.GoldOffenseUserId);
            var gdUser = await _usersRepository.GetUserWithId(game.GoldDefenseUserId);
            var boUser = await _usersRepository.GetUserWithId(game.BlackOffenseUserId);
            var bdUser = await _usersRepository.GetUserWithId(game.BlackDefenseUserId);

            // It's weird, but we store a player's skill in two places - on the User object, and on the stats objects.
            // It's a small performance optimization that also makes it a lot easier to write the code for skill calcs.
            // First, we store it on the user. Then, when calculating their stats we just transfer it over to there.
            await UpdatePlayerSkills(game, goUser, gdUser, boUser, bdUser);

            await RecalculateStatsForUserWithId(goUser, game.EndTimeUtc);
            await RecalculateStatsForUserWithId(boUser, game.EndTimeUtc);

            if (game.IsDoubles)
            {
                await RecalculateStatsForUserWithId(gdUser, game.EndTimeUtc);
                await RecalculateStatsForUserWithId(bdUser, game.EndTimeUtc);
            }
        }

        public async Task UpdatePlayerSkills(Game game, User goUser, User gdUser, User boUser, User bdUser)
        {
            if (game.IsDoubles)
            {
                if (game.GetWinningTeam() == TeamColor.GOLD)
                {
                    _skillCalculator.CalculateNewRatings2V2(goUser, gdUser, boUser, bdUser);
                }
                else
                {
                    _skillCalculator.CalculateNewRatings2V2(boUser, bdUser, goUser, gdUser);
                }

                await _usersRepository.Update2V2SkillForUser(goUser.Id, goUser.Skill2V2);
                await _usersRepository.Update2V2SkillForUser(boUser.Id, boUser.Skill2V2);
                await _usersRepository.Update2V2SkillForUser(gdUser.Id, gdUser.Skill2V2);
                await _usersRepository.Update2V2SkillForUser(bdUser.Id, bdUser.Skill2V2);
            }
            else
            {
                if (game.GetWinningTeam() == TeamColor.GOLD)
                {
                    _skillCalculator.CalculateNewRatings1V1(goUser, boUser);
                }
                else
                {
                    _skillCalculator.CalculateNewRatings1V1(boUser, goUser);
                }

                await _usersRepository.Update1V1SkillForUser(goUser.Id, goUser.Skill1V1);
                await _usersRepository.Update1V1SkillForUser(boUser.Id, boUser.Skill1V1);
            }
        }

        /// <summary>
        /// Concrete implementation of <see cref="IStatsCalculator.RecalculateStatsForUserWithId(User, DateTime)"./>
        /// </summary>
        public async Task RecalculateStatsForUserWithId(User user, DateTime timestamp)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            var userGames = await _gamesRepository.GetListOfRecentGamesByUserId(user.Id, timestamp);
            var user1V1Games = userGames.Where(g => !g.IsDoubles && !g.IsInProgress);
            var user2V2Games = userGames.Where(g => g.IsDoubles && !g.IsInProgress);

            var userStats = new UserStats
            {
                UserId = user.Id,
                Timestamp = timestamp,
                StatsOverall = CalculateOverallStatsForUserWithId(user, userGames.ToImmutableList()),
                Stats1V1 = Calculate1V1StatsForUserWithId(user, user1V1Games.ToImmutableList()),
                Stats2V2 = Calculate2V2StatsForUserWithId(user, user2V2Games.ToImmutableList())
            };

            Log.Logger.Information("Calculated stats for user {@userId}", user.Id);
            
            await _statsRepository.InsertOne(userStats);
        }

        /// <summary>
        /// Calculates a complete 1v1 stats object for the given user, given the list of games to base the stats on.
        /// </summary>
        public Stats1V1 Calculate1V1StatsForUserWithId(User user, IReadOnlyCollection<Game> userGames)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var stats1V1 = new Stats1V1();
            
            WinLossStats.CalculateWinsAndLosses(user.Id, userGames, out var wins, out var losses);
            stats1V1.GamesWon = wins;
            stats1V1.GamesLost = losses;
            stats1V1.Winrate = WinLossStats.CalculateWinrate(wins, losses);
            
            GoalStats.CalculateGoals(user.Id, userGames, out var goals, out _, out var goalsAllowed);
            stats1V1.GoalsScored = goals;
            stats1V1.GoalsAllowed = goalsAllowed;

            stats1V1.Skill = user.Skill1V1;

            return stats1V1;
        }

        /// <summary>
        /// Calculates a complete 2v2 user stats object for the given user based on the given list of games.
        /// </summary>
        public Stats2V2 Calculate2V2StatsForUserWithId(User user, IReadOnlyCollection<Game> userGames)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var stats2V2 = new Stats2V2();

            WinLossStats.CalculateWinsAndLosses(user.Id, userGames, out var wins, out var losses);
            stats2V2.GamesWon = wins;
            stats2V2.GamesLost = losses;
            stats2V2.Winrate = WinLossStats.CalculateWinrate(wins, losses);

            // Calculate the winrates as offense and as defense in addition to the overall winrate
            var offenseGames = userGames.Select(g => g).Where(g => g.GetPlayerPosition(user.Id) == Position.Offense);
            var defenseGames = userGames.Select(g => g).Where(g => g.GetPlayerPosition(user.Id) == Position.Defense);
            WinLossStats.CalculateWinsAndLosses(user.Id, offenseGames, out int offenseWins, out int offenseLosses);
            WinLossStats.CalculateWinsAndLosses(user.Id, defenseGames, out int defenseWins, out int defenseLosses);
            stats2V2.OffenseWins = offenseWins;
            stats2V2.DefenseWins = defenseWins;
            stats2V2.OffenseWinrate = WinLossStats.CalculateWinrate(offenseWins, offenseLosses);
            stats2V2.DefenseWinrate = WinLossStats.CalculateWinrate(defenseWins, defenseLosses);

            OverallStats.OffAndDefGamesCount(user.Id, userGames, out var defs, out var offs);
            stats2V2.GamesAsDefense = defs;
            stats2V2.GamesAsOffense = offs;

            GoalStats.CalculateOffenseDefenseGoals(user.Id, userGames, out var goalsAsDefense, out var goalsAsOffense);
            stats2V2.GoalsScoredAsDefense = goalsAsDefense;
            stats2V2.GoalsScoredAsOffense = goalsAsOffense;
            
            PartnerStats.CalculateMostFrequentTeamMate(user.Id, userGames, out var teamMemberId, out var gamesTogether);
            stats2V2.MostFrequentPartnerId = teamMemberId;
            stats2V2.MostFrequentPartnerNumGamesPlayed = gamesTogether;
            
            PartnerStats.CalculateBestTeamMate(user.Id, userGames, out teamMemberId, out var winRate);
            stats2V2.BestPartnerId = teamMemberId;
            stats2V2.BestPartnerWinrate = winRate;
            
            PartnerStats.CalculateWorstTeamMate(user.Id, userGames, out teamMemberId, out winRate);
            stats2V2.WorstPartnerId = teamMemberId;
            stats2V2.WorstPartnerWinrate = winRate;

            stats2V2.Skill = user.Skill2V2;
            
            return stats2V2;
        }

        /// <summary>
        /// Calculates a complete overall stats object for a given user based on the given list of games.
        /// </summary>
        public StatsOverall CalculateOverallStatsForUserWithId(User user, IReadOnlyList<Game> userGames)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var statsOverall = new StatsOverall();

            WinLossStats.CalculateWinsAndLosses(user.Id, userGames, out var wins, out var losses);
            statsOverall.ShutoutWins = WinLossStats.CalculateShutoutWins(user.Id, userGames);
            statsOverall.GamesWon = wins;
            statsOverall.GamesLost = losses;
            statsOverall.Winrate = WinLossStats.CalculateWinrate(wins, losses);

            OverallStats.CalculateGamesAsGoldAndBlack(user.Id, userGames, out var gamesAsGold, out var gamesAsBlack);
            statsOverall.GamesAsGold = gamesAsGold;
            statsOverall.GamesAsBlack = gamesAsBlack;

            GoalStats.CalculateGoals(user.Id, userGames, out var goals, out var ownGoals, out var goalsAllowed);
            statsOverall.GoalsAllowed = goalsAllowed;
            statsOverall.GoalsScored = goals;
            statsOverall.OwnGoals = ownGoals;

            statsOverall.ShortestGameLengthSecs = GameLength.GetShortestGame(userGames).GetDurationInSeconds();
            statsOverall.LongestGameLengthSecs = GameLength.GetLongestGame(userGames).GetDurationInSeconds();
            statsOverall.AverageGameLengthSecs = GameLength.GetAverageGameLengthSecs(userGames);
            statsOverall.TotalTimePlayedSecs = GameLength.GetTotalTimePlayedSecs(userGames);

            statsOverall.LongestWinStreak = WinLossStreak.CalculateLongestWinStreak(user.Id, userGames);
            statsOverall.LongestLossStreak = WinLossStreak.CalculateLongestLossStreak(user.Id, userGames);

            statsOverall.GoalsPerMinute = 1.0 * statsOverall.GoalsScored / (statsOverall.TotalTimePlayedSecs / 60.0);
            
            return statsOverall;
        }
    }
}
