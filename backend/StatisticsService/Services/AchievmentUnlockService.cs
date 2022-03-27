using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Repositories;
using YouFoos.SharedLibrary.Resources.Strings;

namespace YouFoos.StatisticsService.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="IAchievementUnlockService"/>.
    /// </summary>
    public class AchievementUnlockService : IAchievementUnlockService
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly IAchievementsRepository _achievementsRepository;
        private readonly IAccoladesRepository _accoladesRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public AchievementUnlockService(
            IGamesRepository gamesRepository, 
            IAchievementsRepository achievementsRepository,
            IAccoladesRepository accoladesRepository)
        {
            _gamesRepository = gamesRepository;
            _achievementsRepository = achievementsRepository;
            _accoladesRepository = accoladesRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IAchievementUnlockService.UpdateAchievementStatusesPostGame(Guid)"/>.
        /// </summary>
        public async Task UpdateAchievementStatusesPostGame(Guid gameId)
        {
            Game gameThatJustEnded = await _gamesRepository.GetGameById(gameId);

            foreach (var playerId in gameThatJustEnded.GetPlayerIds())
            {
                await UpdateAchievementsProgressForUser(playerId, gameThatJustEnded);
            }
        }

        /// <summary>
        /// The main function for triggering a progress update on achievements for a given user.
        /// 
        /// This function should be called for each player after a game finishes.
        /// Progress on achievements will be updated and if the criteria for any have been met, 
        /// the achievement will be unlocked.
        /// </summary>
        /// <param name="userId">The ID of the user to check achievement progress for.</param>
        /// <param name="mostRecentGame">The most recent game the user has played in.</param>
        protected async Task UpdateAchievementsProgressForUser(string userId, Game mostRecentGame)
        {
            var currentAchievementsProgress = await _achievementsRepository.GetAllAchievementsForUser(userId);

            var updatedStatuses = new List<AchievementStatus>();

            foreach (string achievementName in AchievementNames.GetAll())
            {
                AchievementStatus currentProgress = currentAchievementsProgress
                    .Where(ach => ach.Name == achievementName)
                    .SingleOrDefault();

                AchievementStatus updatedStatus = achievementName switch
                {
                    // These achievements don't require any additional database checks at all and don't depend on past status.
                    AchievementNames.ThankUNext => CheckThankUNextAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.SlowRoller => CheckSlowRollerAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.TheBestOffense => CheckBestOffenseAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.ComebackKing => CheckComebackKingAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.ReproducableBug => CheckReproducableBugAchievement(mostRecentGame, userId, currentProgress),

                    // These achievements don't require any additional database checks but do depend on past status of the achievement.
                    AchievementNames.IWasntReady => CheckIWasntReadyAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.Seppuku => CheckSeppukuAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.SoreBack => CheckSoreBackAchievement(mostRecentGame, userId, currentProgress),

                    // These ones don't require additional db checks but do depend on past status and must be updated EVERY game.
                    AchievementNames.OnARoll => CheckOnARollAchievement(mostRecentGame, userId, currentProgress),
                    AchievementNames.Penultimate => CheckPenultimateAchievement(mostRecentGame, userId, currentProgress),

                    // These ones DO have additional DB checks and absolutely must be checked every game before the next game starts.
                    AchievementNames.LookMom => await CheckLookMomAchievement(userId, currentProgress),
                    AchievementNames.KingOfTheWorld => await CheckKingOfTheWorldAchievement(userId, currentProgress),
                    AchievementNames.ItsNotAnAddiction => await CheckItsNotAnAddictionAchievement(mostRecentGame, userId, currentProgress),

                    // If we get an unexpected achievement title, blow up
                    _ => throw new ArgumentOutOfRangeException(nameof(achievementName))
                };

                updatedStatus.RecalculateProgress(mostRecentGame.EndTimeUtc);
                updatedStatuses.Add(updatedStatus);
            }

            await _achievementsRepository.UpdateAchievementsProgressForUser(updatedStatuses);

            Log.Information("Reculated achievements for user {@userId}", userId);
        }

        protected AchievementStatus CheckIWasntReadyAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.IWasntReady, eventsNeeded: 20);
            
            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            int playerGoalsFromFirst10Seconds = mostRecentGame.Goals
                .Where(goal => !goal.IsOwnGoal)
                .Where(goal => !goal.IsUndone)
                .Where(goal => goal.ScoringUserId == playerId)
                .Where(goal => goal.TimeStampGameClock <= 5)
                .Count();

            currentStatus.NumQualifyingEvents += playerGoalsFromFirst10Seconds;

            return currentStatus;
        }

        protected AchievementStatus CheckSlowRollerAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.SlowRoller, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            bool playerWonGame = mostRecentGame.GetWinningTeam() == mostRecentGame.GetPlayerTeam(playerId);
            bool gameWasLongerThan10Minutes = mostRecentGame.GetDurationInSeconds() > TimeSpan.FromMinutes(10).TotalSeconds;

            if (playerWonGame && gameWasLongerThan10Minutes)
            {
                currentStatus.NumQualifyingEvents += 1;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckSeppukuAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.Seppuku, eventsNeeded: 3);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            bool playerWasOnLosingTeam = mostRecentGame.GetPlayerTeam(playerId) != mostRecentGame.GetWinningTeam();
            Goal finalGoalInGame = mostRecentGame.Goals.OrderByDescending(goal => goal.TimeStampGameClock).First();
            bool playerScoredFinalGoal = finalGoalInGame.ScoringUserId == playerId;

            if (playerWasOnLosingTeam && playerScoredFinalGoal && finalGoalInGame.IsOwnGoal)
            {
                currentStatus.NumQualifyingEvents++;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckThankUNextAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.ThankUNext, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            if (mostRecentGame.GetDurationInSeconds() < 90)
            {
                currentStatus.NumQualifyingEvents += 1;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckBestOffenseAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.TheBestOffense, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;
            if (mostRecentGame.GetPlayerTeam(playerId) != mostRecentGame.GetWinningTeam()) { return currentStatus; }
            if (!mostRecentGame.IsDoubles) { return currentStatus; } 

            int numGoalsScoredByPlayer = mostRecentGame.Goals
                .Where(goal => goal.ScoringUserId == playerId)
                .Where(goal => goal.IsUndone == false)
                .Where(goal => goal.IsOwnGoal == false)
                .Count();

            bool playerWasDefense = mostRecentGame.GetPlayerPosition(playerId) == Position.Defense;
            bool playerScoredAllTheGoals = numGoalsScoredByPlayer == Game.NumGoalsNeededToWin;

            if (playerWasDefense && playerScoredAllTheGoals)
            {
                currentStatus.NumQualifyingEvents += 1;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckPenultimateAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.Penultimate, eventsNeeded: 3);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            TeamColor playerTeam = mostRecentGame.GetPlayerTeam(playerId);
            int playerTeamScore = mostRecentGame.GetTeamScore(playerTeam);

            // If the player lost by one point, increment their progress - else reset it.
            if (playerTeamScore == Game.NumGoalsNeededToWin - 1)
            {
                currentStatus.NumQualifyingEvents++;
            }
            else 
            {
                currentStatus.NumQualifyingEvents = 0;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckComebackKingAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.ComebackKing, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;
            if (mostRecentGame.GetWinningTeam() != mostRecentGame.GetPlayerTeam(playerId)) return currentStatus;

            // In order for this to be a comeback, the first MAX_SCORE-1 goals in a row should be scored by the other team.
            var realGoals = mostRecentGame.Goals.Where(goal => !goal.IsUndone).ToList();
            for (int goal = 0; goal < (Game.NumGoalsNeededToWin - 1); goal++)
            {
                TeamColor playerTeam = mostRecentGame.GetPlayerTeam(playerId);
                if (realGoals[goal].TeamScoredAgainst != playerTeam)
                {
                    return currentStatus;
                }
            }

            // If we make it here, the user has met the criteria to unlock the achievement
            currentStatus.NumQualifyingEvents++;

            return currentStatus;
        }

        protected AchievementStatus CheckOnARollAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.OnARoll, eventsNeeded: 10);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            if (mostRecentGame.GetWinningTeam() == mostRecentGame.GetPlayerTeam(playerId))
            {
                currentStatus.NumQualifyingEvents++;
            }
            else
            {
                // If they didn't win this game, reset their streak.
                currentStatus.NumQualifyingEvents = 0;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckReproducableBugAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.ReproducableBug, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            List<Goal> goalsToCheck = mostRecentGame.Goals
                .Where(goal => !goal.IsUndone)
                .Where(goal => !goal.IsOwnGoal)
                .Where(goal => goal.ScoringUserId == playerId)
                .ToList();

            int numGoalsScoredByPlayer = goalsToCheck.Count;

            if (numGoalsScoredByPlayer < 3) return currentStatus;

            int firstGoalToCheck = 0;
            int secondGoalToCheck = 2;

            while (secondGoalToCheck < numGoalsScoredByPlayer)
            {
                // If the first and the third goal in a sequence are below 15 seconds apart, they get the achievement.
                long firstGoalTime = goalsToCheck[firstGoalToCheck].TimeStampGameClock;
                long secondGoalTime = goalsToCheck[secondGoalToCheck].TimeStampGameClock;

                if ((secondGoalTime - firstGoalTime) < 15)
                {
                    currentStatus.NumQualifyingEvents++;
                    return currentStatus;
                }

                firstGoalToCheck++;
                secondGoalToCheck++;
            }

            return currentStatus;
        }

        protected AchievementStatus CheckSoreBackAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.SoreBack, eventsNeeded: 3);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            int numGoalsScoredByPlayer = mostRecentGame.Goals
                .Where(goal => goal.ScoringUserId == playerId)
                .Where(goal => goal.IsUndone == false)
                .Where(goal => goal.IsOwnGoal == false)
                .Count();

            if (numGoalsScoredByPlayer == Game.NumGoalsNeededToWin)
            {
                currentStatus.NumQualifyingEvents++;
            }

            return currentStatus;
        }

        protected async Task<AchievementStatus> CheckItsNotAnAddictionAchievement(Game mostRecentGame, string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.ItsNotAnAddiction, eventsNeeded: 5);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            // This achievement looks for a 5 day in a row play streak. The easiest way to check that is to just check if they played yesterday.
            // We can use that information to keep a running streak and reset it if we find they didn't play yeterday.
            var gamesFromYesterday = await _gamesRepository.GetListOfRecentGamesByUserId(
                playerId,
                cutoff: mostRecentGame.EndTimeUtc.Date,
                numberOfGames: 1
            );

            var cutoff = mostRecentGame.EndTimeUtc.Date.AddDays(1);

            // There is one caveat - if the user has already played today, don't increment again
            // If there are 2 games from today, we know they've already played today
            var gamesFromToday = await _gamesRepository.GetListOfRecentGamesByUserId(
                playerId,
                cutoff: cutoff,
                numberOfGames: 2
            );

            bool playedYesterday = gamesFromYesterday != null && gamesFromYesterday.Any();
            bool alreadyCountedForToday = gamesFromToday != null && gamesFromToday.Count > 1
                && gamesFromToday[1].EndTimeUtc > cutoff.AddDays(-1);

            if (playedYesterday)
            {
                if (!alreadyCountedForToday)
                {
                    currentStatus.NumQualifyingEvents++;
                }
            }
            else
            {
                currentStatus.NumQualifyingEvents = 1;
            }

            return currentStatus;
        }

        protected async Task<AchievementStatus> CheckKingOfTheWorldAchievement(string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.KingOfTheWorld, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            List<Accolade> currentHallOfFame = await _accoladesRepository.GetAccolades();

            foreach (Accolade accolade in currentHallOfFame)
            {
                if (accolade.Name == AccoladeTitles.HighestRank && accolade.UserId == playerId)
                {
                    currentStatus.NumQualifyingEvents++;
                }
            }

            return currentStatus;
        }

        protected async Task<AchievementStatus> CheckLookMomAchievement(string playerId, AchievementStatus currentStatus)
        {
            currentStatus ??= new AchievementStatus(playerId, AchievementNames.LookMom, eventsNeeded: 1);

            if (currentStatus.UnlockedDateTime != null) return currentStatus;

            List<Accolade> currentHallOfFame = await _accoladesRepository.GetAccolades();

            foreach (Accolade accolade in currentHallOfFame)
            {
                if (accolade.UserId == playerId)
                {
                    currentStatus.NumQualifyingEvents++;
                }
            }

            return currentStatus;
        }
    }
}
