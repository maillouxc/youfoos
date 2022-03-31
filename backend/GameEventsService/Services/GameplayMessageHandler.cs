using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Enums;
using YouFoos.DataAccess.Repositories;
using YouFoos.GameEventsService.Messages;
using YouFoos.Exceptions;

namespace YouFoos.GameEventsService.Services
{
    /// <summary>
    /// This class is responsible for processing RabbitMQ messages received on the GameEvents channel.
    /// </summary>
    public class GameplayMessageHandler : IGameplayMessageHandler
    {
        private readonly IRfidToUserAccountResolverService _rfidToUserAccountResolverService;
        private readonly IGameToTournamentResolverService _tournamentResolver;
        private readonly IGamesRepository _gamesRepository;

        public GameplayMessageHandler(IRfidToUserAccountResolverService rfidToUserAccountResolverService,
                                      IGameToTournamentResolverService gameToTournamentResolverService,
                                      IGamesRepository gamesRepository)
        {
            _rfidToUserAccountResolverService = rfidToUserAccountResolverService;
            _tournamentResolver = gameToTournamentResolverService;
            _gamesRepository = gamesRepository;
        }
        
        /// <summary>
        /// Parses messages, then delivers them to their correct specific handlers. 
        /// </summary>
        public async Task HandleMessageAsync(string messageText)
        {
            object parsedGameEventMessage;
            string type = GetMessageType(messageText);
            switch (type)
            {
                case "gameStart":
                    parsedGameEventMessage = JsonConvert.DeserializeObject<GameStartMessage>(messageText);
                    Log.Logger.Information("Game start message received: {@message}", parsedGameEventMessage);
                    await HandleGameStartMessage((GameStartMessage) parsedGameEventMessage);
                    break;
                case "gameEnd":
                    parsedGameEventMessage = JsonConvert.DeserializeObject<GameEndMessage>(messageText);
                    Log.Logger.Information("Game end message received: {@message}", parsedGameEventMessage);
                    await HandleGameEndMessage((GameEndMessage) parsedGameEventMessage);
                    break;
                case "goalScored":
                    parsedGameEventMessage = JsonConvert.DeserializeObject<GoalScoredMessage>(messageText);
                    Log.Logger.Information("Goal scored message received: {@message}", parsedGameEventMessage);
                    await HandleGoalScoredMessage((GoalScoredMessage) parsedGameEventMessage);
                    break;
                case "goalUndo":
                    parsedGameEventMessage = JsonConvert.DeserializeObject<GoalUndoMessage>(messageText);
                    Log.Logger.Information("Goal undo message received: {@message}", parsedGameEventMessage);
                    await HandleGoalUndoMessage((GoalUndoMessage) parsedGameEventMessage);
                    break;
                default:
                    throw new InvalidMessageTypeException("Invalid message type: " + type);
            }
        }

        /// <summary>
        /// Creates a new in-progress game in the database if one does not already exist with the given GUID.
        /// If there is already a game in progress, it is deleted.
        /// </summary>
        public async Task HandleGameStartMessage(GameStartMessage message)
        {
            // If we ever want multi-table support, we will have to change this
            await DeleteExistingGameInProgress();

            var game = new Game(Guid.Parse(message.GameGuid))
            {
                IsDoubles = (message.GameType.Equals(GameTypes.Doubles)),
                StartTimeUtc = message.Timestamp,

                BlackOffenseUserId = (await _rfidToUserAccountResolverService
                    .GetUserWithRfidOrCreateNewAnonymousUser(message.BlackOffenseRfid)).Id,
                BlackDefenseUserId = (await _rfidToUserAccountResolverService
                    .GetUserWithRfidOrCreateNewAnonymousUser(message.BlackDefenseRfid)).Id,
                GoldOffenseUserId = (await _rfidToUserAccountResolverService
                    .GetUserWithRfidOrCreateNewAnonymousUser(message.GoldOffenseRfid)).Id,
                GoldDefenseUserId = (await _rfidToUserAccountResolverService
                    .GetUserWithRfidOrCreateNewAnonymousUser(message.GoldDefenseRfid)).Id
            };

            // If this game has the right players to be considered a tournament game, get the tournament id.
            game.TournamentId = await _tournamentResolver.CheckIsTournamentGame(game);

            Log.Logger.Debug("Adding game to database: {@game}", game);
            await _gamesRepository.InsertGame(game);
        }

        private async Task DeleteExistingGameInProgress()
        {
            var gameInProgress = await _gamesRepository.GetCurrentGame();
            if (gameInProgress != null)
            {
                Log.Logger.Information("Deleting existing game in progress to start new game");
                await _gamesRepository.DeleteGameByIdAsync(gameInProgress.Guid);
            }
        }

        /// <summary>
        /// Creates a new goal and inserts it into the goals collection of the appropriate game in the database.
        /// If the game is not found in the database, the method has no effect.
        /// </summary>
        public async Task HandleGoalScoredMessage(GoalScoredMessage message)
        {
            var guid = Guid.Parse(message.GameGuid);
            var scoringPlayer = await _rfidToUserAccountResolverService.GetUserWithRfid(message.ScoringPlayerRfid);
            var game = await _gamesRepository.GetGameById(guid);

            if (game == null)
            {
                throw new YouFoosException("No game in progress to score goal for.");
            }

            var scoringTeam = game.GetPlayerTeam(scoringPlayer.Id);

            var goal = new Goal(guid)
            {
                ScoringUserId = scoringPlayer.Id,
                TeamScoredAgainst = message.TeamScoredAgainst,
                IsOwnGoal = scoringTeam == message.TeamScoredAgainst,
                TimeStampUtc = message.Timestamp,
                TimeStampGameClock = message.RelativeTimestamp
            };

            Log.Logger.Debug("Adding goal to {@Game}: {@Goal}", game, goal);
            await _gamesRepository.InsertGoal(goal);
        }
        
        /// <summary>
        /// Undoes the appropriate goal in the games database.
        /// </summary>
        public async Task HandleGoalUndoMessage(GoalUndoMessage message)
        {
            var gameInProgress = await _gamesRepository.GetCurrentGame();
            if (gameInProgress == null)
            {
                throw new YouFoosException("Cannot undo a goal if there is no game in progress");
            }

            await _gamesRepository.UndoGoal(Guid.Parse(message.GameGuid), message.Timestamp);
        }

        /// <summary>
        /// Sets the appropriate game in the database to finished.
        /// </summary>
        public async Task HandleGameEndMessage(GameEndMessage message)
        {
            var gameInProgress = await _gamesRepository.GetCurrentGame();
            if (gameInProgress == null)
            {
                throw new YouFoosException("No game in progress available to be ended");
            }

            await _gamesRepository.EndGame(Guid.Parse(message.GameGuid), message.Timestamp);
        }

        /// <summary>
        /// Returns the type of the given message, either "gameStart", "gameEnd", "goalScored", or "goalUndo".
        /// </summary>
        public static string GetMessageType(string message)
        {
            var jsonObject = (JObject)JsonConvert.DeserializeObject(message);
            return jsonObject["type"].Value<string>();
        }
    }
}
