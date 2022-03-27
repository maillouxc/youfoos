using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouFoos.Api.Dtos;
using YouFoos.Api.Exceptions;
using YouFoos.DataAccess.Entities;
using YouFoos.DataAccess.Entities.Account;
using YouFoos.DataAccess.Entities.Tournaments;
using YouFoos.DataAccess.Repositories;
using YouFoos.SharedLibrary.Exceptions;

namespace YouFoos.Api.Services
{
    /// <summary>
    /// Concrete implementation of <see cref="ITournamentsService"/>.
    /// </summary>
    public class TournamentsService : ITournamentsService
    {
        private readonly IEmailSender _emailSender;

        private readonly ITournamentsRepository _tournamentsRepository;
        private readonly IUsersRepository _usersRepository;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TournamentsService(
            IEmailSender emailSender,
            ITournamentsRepository tournamentsRepository,
            IUsersRepository usersRepository)
        {
            _emailSender = emailSender;
            _tournamentsRepository = tournamentsRepository;
            _usersRepository = usersRepository;
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.GetCurrentTournament"/>.
        /// </summary>
        public async Task<Tournament> GetCurrentTournament()
        {
            return await _tournamentsRepository.GetCurrentTournament();
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.GetTournamentById(Guid)"/>.
        /// </summary>
        public async Task<Tournament> GetTournamentById(Guid id)
        {
            return await _tournamentsRepository.GetTournamentById(id);
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.GetRecentTournaments(int, int)"/>.
        /// </summary>
        public async Task<PaginatedResult<Tournament>> GetRecentTournaments(int pageSize, int pageNumber)
        {
            return await _tournamentsRepository.GetRecentTournaments(pageSize, pageNumber);
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.CreateTournament(CreateTournamentRequest, string)"/>.
        /// </summary>
        public async Task<Tournament> CreateTournament(CreateTournamentRequest request, string creatorId)
        {
            var currentlyInProgressTournament = _tournamentsRepository.GetCurrentTournament();
            
            if (currentlyInProgressTournament != null)
            {
                throw new YouFoosException("Only one tournament can be scheduled or in progress at a time.");
            }

            var tournament = new Tournament(
                request.Name,
                request.GameType,
                request.PlayerCount,
                request.StartDate,
                request.EndDate,
                creatorId
            );

            tournament.Description = request.Description;

            await _tournamentsRepository.InsertOne(tournament);

            // Since we often test with real email addresses, we don't want to send them out in dev mode.
            #if !DEBUG
                List<string> allUserEmails = await _usersRepository.GetAllUserEmails();
                foreach (string userEmail in allUserEmails)
                {
                    // We don't want to halt this method, even if this fails - just fire and forget
                    _ = _emailSender.SendTournamentCreationEmail(request, userEmail);
                }
            #endif

            // MongoDB will automatically set the ID of the tournament object in C# once inserted.
            return tournament;

        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.RegisterForTournament(Guid, string)"/>.
        /// </summary>
        public async Task RegisterForTournament(Guid tournamentId, string userId)
        {
            Tournament tournament = await _tournamentsRepository.GetTournamentById(tournamentId);

            if (tournament == null)
            {
                throw new ResourceNotFoundException("Tournament not found with the given ID.");
            }
            if (tournament.CurrentState != TournamentState.Registration)
            {
                throw new YouFoosException("This tournament is not open for registration.");
            }

            // If the player is already in the tournament, this will throw an exception.
            tournament.PlayerIds.Add(userId);

            // If the tournament is full now that this player has joined, update the tournament state.
            if (tournament.PlayerIds.Count == tournament.PlayerCount)
            {
                tournament.CurrentState = TournamentState.Seeding;
            }

            await _tournamentsRepository.UpdateTournament(tournament);

            if (tournament.CurrentState == TournamentState.Seeding)
            {
                await SeedTournament(tournament);
            }
        }

        /// <summary>
        /// Concrete implementation of <see cref="ITournamentsService.UpdateTournament"/>.
        /// </summary>
        public Task UpdateTournament()
        {
            // TODO

            throw new NotImplementedException();
        }

        public async Task SeedTournament(Tournament tournament)
        {
            var allPlayersInTournament = new List<User>();

            foreach (var userId in tournament.PlayerIds)
            {
                var player = await _usersRepository.GetUserWithId(userId);
                allPlayersInTournament.Add(player);
            }

            var tournamentSeeder = new HiLoTournamentSeeder();
            tournamentSeeder.SeedTournament(tournament, allPlayersInTournament);

            tournament.CurrentState = TournamentState.WaitingForStartTime;

            await _tournamentsRepository.UpdateTournament(tournament);
        }
    }
}
