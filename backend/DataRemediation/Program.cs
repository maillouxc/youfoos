using System;
using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using YouFoos.DataAccess.Config;
using YouFoos.StatisticsService.Services;

namespace YouFoos.DataRemediation
{
    public class Program
    {
        public static IConfigurationRoot Configuration;

        private static void Main()
        {
            Configuration = LoadConfig();

            var serviceProvider = PrepareDependencyInjection();

            Log.Logger.Information("Starting data remediation service");

            var dataRemediator = serviceProvider.GetService<DataRemediator>();

            try
            {
                Console.WriteLine("DANGER: Running this tool WILL DESTROY DATA.");
                Console.WriteLine("Please manually backup the entire database.");
                Console.WriteLine("Press ENTER when you have completed the backup or CTRL + C to exit.");
                Console.ReadLine();

                Console.WriteLine("Before remediation we must reset all player ranks + press ENTER to begin.");
                Console.ReadLine();
                dataRemediator.ResetAllPlayerRanks().GetAwaiter().GetResult();

                Console.WriteLine("We need to delete the stats, accolades, and achievements collection. Press ENTER to do so.");
                Console.ReadLine();
                dataRemediator.DropCollectionsThatWillBeRebuilt().GetAwaiter().GetResult();

                Console.WriteLine("Ready to remediate data - press ENTER to continue.");
                Console.ReadLine();
                dataRemediator.RemediateData().GetAwaiter().GetResult();

                Console.WriteLine("Remediation complete - press ENTER to exit");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Log.Logger.Error("Failed to remediate data - {@ex}", e);
            }

            Log.Logger.Information("Data remediation complete");
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
        }

        /// <summary>
        /// This method should be used to register service classes for DI with Autofac.
        /// </summary>
        public static void RegisterServicesForDependencyInjection(ContainerBuilder builder)
        {
            builder.RegisterType<DataRemediator>().SingleInstance();
            builder.RegisterType<StatsCalculator>().As<IStatsCalculator>();
            builder.RegisterType<AccoladesCalculator>().As<IAccoladesCalculator>();
            builder.RegisterType<AchievementUnlockService>().As<IAchievementUnlockService>();
            builder.RegisterType<TrueskillCalculator>().As<ITrueskillCalculator>();
        }
    }
}
