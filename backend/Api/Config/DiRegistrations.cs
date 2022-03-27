using System.Diagnostics.CodeAnalysis;
using Autofac;
using YouFoos.Api.Services;
using YouFoos.Api.Services.Authentication;
using YouFoos.Api.Services.Users;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// This class simply holds a static method to configure Autofac DI registrations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DiRegistrations
    {
        /// <summary>
        /// Generic helper method called to register all the needed dependency injections with the ContainerBuilder.
        /// </summary>
        public static void RegisterStuffForDependencyInjection(ContainerBuilder builder)
        {
            RegisterServicesForDependencyInjection(builder);
            RegisterRepositoriesForDependencyInjection(builder);
        }

        /// <summary>
        /// This method is where we need to register our service classes for dependency injection with Autofac.
        /// </summary>
        private static void RegisterServicesForDependencyInjection(ContainerBuilder builder)
        {
            builder.RegisterType<Authenticator>().As<IAuthenticator>();
            builder.RegisterType<JwtMinter>().As<IJwtMinter>();
            builder.RegisterType<AccountCreationService>().As<IAccountCreationService>();
            builder.RegisterType<PasswordChangeService>().As<IPasswordChangeService>();
            builder.RegisterType<EmailSender>().As<IEmailSender>();
            builder.RegisterType<RfidChangeService>().As<IRfidChangeService>();
            builder.RegisterType<AccoladesService>().As<IAccoladesService>();
            builder.RegisterType<StatsService>().As<IStatsService>();
            builder.RegisterType<AvatarService>().As<IAvatarService>();
            builder.RegisterType<GamesService>().As<IGamesService>();
            builder.RegisterType<UsersService>().As<IUsersService>();
            builder.RegisterType<AchievementsService>().As<IAchievementsService>();
            builder.RegisterType<TournamentsService>().As<ITournamentsService>();
        }

        /// <summary>
        /// This function is just a wrapper for the DataAccess method with the same
        /// name. It exists only to clean up the code in Startup.cs, and keep a more
        /// uniform interface. The startup class shouldn't need to know about the DataAccess classes.
        /// </summary>
        private static void RegisterRepositoriesForDependencyInjection(ContainerBuilder builder)
        {
            DataAccess.Config.AutofacDiRegistrations.RegisterRepositoriesForDependencyInjection(builder);
        }
    }
}
