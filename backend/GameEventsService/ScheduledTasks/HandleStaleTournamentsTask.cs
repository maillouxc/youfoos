using FluentScheduler;
using Serilog;
using System;
using System.Threading.Tasks;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Tournaments;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.GameEventsService.ScheduledTasks
{
    // TODO finish this file documentation

    public class HandleStaleTournamentsTask : IJob
    {
        private readonly ITournamentsRepository _tournamentsRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HandleStaleTournamentsTask(ITournamentsRepository tournamentsRepository)
        {
            _tournamentsRepository = tournamentsRepository;
        }

        public async void Execute()
        {
            Log.Logger.Information("Executing scheduled task to handle stale tournaments");

            Tournament currentTournament = await _tournamentsRepository.GetCurrentTournament();

            if (currentTournament == null)
            {
                Log.Logger.Information("There is no current or scheduled tournament - no need to do anything");
                return;
            }

            await StartTournamentIfTime(currentTournament);
            await HandleTournamentsThatDidntFinishOnTime(currentTournament);
        }

        private async Task StartTournamentIfTime(Tournament tournament)
        {
            bool enoughPlayers = tournament.PlayerIds.Count == tournament.PlayerCount;
            bool hasStartDatePassed = tournament.StartDate < DateTime.UtcNow;
            bool waitingToStart = tournament.CurrentState == TournamentState.WaitingForStartTime;
            bool alreadyHandled = tournament.CurrentState == TournamentState.ErrorNotEnoughPlayers;

            if (waitingToStart && hasStartDatePassed && enoughPlayers)
            {
                Log.Logger.Information("Starting tournament");
                tournament.CurrentState = TournamentState.InProgress;
                await _tournamentsRepository.UpdateTournament(tournament);
                return;
            }

            if (hasStartDatePassed && !enoughPlayers && !alreadyHandled)
            {
                Log.Logger.Information("Handling tournament with not enough players to start");
                tournament.CurrentState = TournamentState.ErrorNotEnoughPlayers;
                await _tournamentsRepository.UpdateTournament(tournament);
                return;
            }
        }

        private async Task HandleTournamentsThatDidntFinishOnTime(Tournament tournament)
        {
            // TODO
        }
    }
}
