using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Serilog;
using YouFoos.DataAccess.Config;
using YouFoos.StatisticsService.Config;
using YouFoos.StatisticsService.Services;

namespace YouFoos.StatisticsService
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static IConfigurationRoot Configuration;

        private static void Main()
        {
            Configuration = LoadConfig();

            var serviceProvider = PrepareDependencyInjection();
      
            Log.Logger.Information("Starting Stats Service...");

            // Connect to RabbitMQ and begin consuming messages
            var rabbitMqMessageListener = serviceProvider.GetService<RabbitMqMessageListener>(); 
            try 
            {
                rabbitMqMessageListener.InitConnection(new ConnectionFactory());
                rabbitMqMessageListener.BeginConsuming();
            }
            catch (Exception ex)
            {
                Log.Logger.Information("Failed to consume RabbitMQ - {@ex}", ex);
            }

            Log.Logger.Information("Service now running.");
            while (true) { } // Run the program in an infinite loop until forcibly stopped
        }

        private static AutofacServiceProvider PrepareDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            RegisterServicesForDependencyInjection(containerBuilder);
            AutofacDiRegistrations.RegisterRepositoriesForDependencyInjection(containerBuilder);
            var diContainer = containerBuilder.Build();
            var serviceProvider = new AutofacServiceProvider(diContainer);

            return serviceProvider;
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
            builder.RegisterType<StatsCalculator>().As<IStatsCalculator>();
            builder.RegisterType<AccoladesCalculator>().As<IAccoladesCalculator>();
            builder.RegisterType<TrueskillCalculator>().As<ITrueskillCalculator>();
            builder.RegisterType<AchievementUnlockService>().As<IAchievementUnlockService>();
            builder.RegisterType<TournamentGameHandler>().As<ITournamentGameHandler>();
            builder.RegisterType<RabbitMqMessageListener>();
        }
    }
}
