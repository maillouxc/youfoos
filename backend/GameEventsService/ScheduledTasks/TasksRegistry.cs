using FluentScheduler;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.GameEventsService.ScheduledTasks
{
    /// <summary>
    /// The <see cref="Registry"/> that is used to configure scheduled tasks to run in the microservice.
    /// </summary>
    public class TasksRegistry : Registry
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public TasksRegistry(IGamesRepository gamesRepository, ITournamentsRepository tournamentsRepository)
        {
            var staleGamesTask = new DeleteStaleGamesTask(gamesRepository);
            Schedule(() => staleGamesTask)
                .ToRunNow()
                .AndEvery(30)
                .Minutes();

            // TODO this is a poor way to handle it - let's rethink it
            var staleTournamentsTask = new HandleStaleTournamentsTask(tournamentsRepository);
            Schedule(() => staleTournamentsTask)
                .ToRunNow()
                .AndEvery(1)
                .Days()
                .At(1, 0);
        }
    }
}
