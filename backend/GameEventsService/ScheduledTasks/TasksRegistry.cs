using System.Diagnostics.CodeAnalysis;
using FluentScheduler;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.GameEventsService.ScheduledTasks
{
    [ExcludeFromCodeCoverage]
    public class TasksRegistry : Registry
    {
        public TasksRegistry(
            IGamesRepository gamesRepository, 
            ITournamentsRepository tournamentsRepository)
        {
            var staleGamesTask = new DeleteStaleGamesTask(gamesRepository);
            Schedule(() => staleGamesTask)
                .ToRunNow()
                .AndEvery(30)
                .Minutes();

            var staleTournamentsTask = new HandleStaleTournamentsTask(tournamentsRepository);
            Schedule(() => staleTournamentsTask)
                .ToRunNow()
                .AndEvery(1)
                .Days()
                .At(1, 0);
        }
    }
}
