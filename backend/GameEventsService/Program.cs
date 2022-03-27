using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentScheduler;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using YouFoos.DataAccess.Config;
using YouFoos.DataAccess.Repositories;
using YouFoos.GameEventsService.Config;
using YouFoos.GameEventsService.ScheduledTasks;
using YouFoos.GameEventsService.Services;

namespace YouFoos.GameEventsService
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static IConfigurationRoot Configuration;

        private static void Main()
        {
            Configuration = LoadConfig();

            // Prepare dependency injection service collection
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // This is the earliest place we can use the logger
            Log.Logger.Information("Starting gameplay event listener service");

            var containerBuilder = new ContainerBuilder();

            // Add everything in the service collection to Autofac
            containerBuilder.Populate(serviceCollection);

            // Now we make our Autofac registrations before we call Populate, then the ones in the
            // ServiceCollection will override the ones for Autofac - else the opposite happens.
            RegisterServicesForDependencyInjection(containerBuilder);
            AutofacDiRegistrations.RegisterRepositoriesForDependencyInjection(containerBuilder);

            // Creating a new AutofacServiceProvider makes the container available to your app
            // using the Microsoft IServiceProvider interface so you can use those abstractions
            // rather than binding directly to Autofac.
            var diContainer = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(diContainer);

            ScheduleRecurringTasks(serviceProvider);

            try
            {
                var gameEventsRabbitConsumer = serviceProvider.GetService<RabbitMqConsumer>();
                gameEventsRabbitConsumer.InitConnection(new ConnectionFactory());
                gameEventsRabbitConsumer.BeginConsuming();
            }
            catch (Exception e)
            {
                Log.Logger.Error("Exception occurred while bringing up RabbitMQ listener: {@ExceptionInfo}", e);
            }
            
            Console.WriteLine("Service now running. Press Ctrl+C to exit.");
            while (true) {} // Run the program in an infinite loop until forcibly stopped
        }

        /// <summary>
        /// Loads configuration info for the program from the appsettings.json file.
        /// </summary>
        public static IConfigurationRoot LoadConfig()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = "Development";
            }

            Console.WriteLine($"Loading configuration for: {environmentName}");

            // Read the appsettings.json config file
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentName}.json", false, true)
                .AddEnvironmentVariables();

            return configBuilder.Build();
        }

        /// <summary>
        /// This is the typical ConfigureServices method used by Microsoft in .NET Core apps.
        /// It's used to register types for the standard built in DI framework.
        /// We aren't using that framework, but sometimes it's convenient to be able to
        /// register things as though we were.
        /// </summary>
        public static void ConfigureServices(IServiceCollection services)
        {
            // Configure the static Serilog logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .CreateLogger();

            // Configure settings objects for DI so that they can be injected anywhere they are needed in the program
            services.Configure<MongoSettings>(Configuration.GetSection("MongoDB"));
            services.Configure<RabbitMqSettings>(Configuration.GetSection("RabbitMq"));
        }

        /// <summary>
        /// This method should be used to register service classes for DI with Autofac.
        /// </summary>
        public static void RegisterServicesForDependencyInjection(ContainerBuilder builder)
        {
            builder.RegisterType<GameplayMessageHandler>().As<IGameplayMessageHandler>();
            builder.RegisterType<RabbitMqConsumer>();
            builder.RegisterType<RfidToUserAccountResolverService>().As<IRfidToUserAccountResolverService>();
            builder.RegisterType<GameToTournamentResolverService>().As<IGameToTournamentResolverService>();
        }

        private static void ScheduleRecurringTasks(AutofacServiceProvider serviceProvider)
        {
            // We have to manually inject dependencies because of library limitations
            var gamesRepo = serviceProvider.GetRequiredService<IGamesRepository>();
            var tournamentsRepo = serviceProvider.GetRequiredService<ITournamentsRepository>();

            var tasksRegistry = new TasksRegistry(gamesRepo, tournamentsRepo);
            JobManager.Initialize(tasksRegistry);
            JobManager.UseUtcTime();

            // We need to capture exceptions that occur in the scheduled jobs
            JobManager.JobException += info => 
                Log.Error("Error in scheduled task: {@ExceptionInfo}", info.Exception);
        }
    }
}
