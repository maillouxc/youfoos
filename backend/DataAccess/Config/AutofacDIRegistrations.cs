using System.Diagnostics.CodeAnalysis;
using Autofac;
using YouFoos.DataAccess.Repositories;

namespace YouFoos.DataAccess.Config
{
    /// <summary>
    /// This class is used to wrap a static method to register all repositories in the DataAccess
    /// project for dependency injection using Autofac.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AutofacDiRegistrations
    {
        /// <summary>
        /// Registers all repository classes in the DataAccess project for Autofac dependency injection.
        /// You can still register them manually and individually from the projects which depend on
        /// DataAccess, but this method provides a more convenient, centralized way to do so that reduces
        /// duplication of code.
        /// </summary>
        /// <param name="builder">The Autofac container builder</param>
        public static void RegisterRepositoriesForDependencyInjection(ContainerBuilder builder)
        {
            builder.RegisterType<MongoContext>().As<IMongoContext>();

            builder.RegisterType<UsersRepository>().As<IUsersRepository>();
            builder.RegisterType<AccountCredentialsRepository>().As<IAccountCredentialsRepository>();
            builder.RegisterType<PasswordResetCodeRepository>().As<IPasswordResetCodeRepository>();
            builder.RegisterType<AvatarsRepository>().As<IAvatarsRepository>();
            builder.RegisterType<GamesRepository>().As<IGamesRepository>();
            builder.RegisterType<StatsRepository>().As<IStatsRepository>();
            builder.RegisterType<AccoladesRepository>().As<IAccoladesRepository>();
            builder.RegisterType<AchievementsRepository>().As<IAchievementsRepository>();
            builder.RegisterType<TournamentsRepository>().As<ITournamentsRepository>();
        }
    }
}
