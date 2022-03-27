using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using YouFoos.DataAccess;
using YouFoos.DataAccess.SharedTestUtils;

namespace YouFoos.StatisticsService.Tests.Integration
{
    [ExcludeFromCodeCoverage]
    public static class TestingDIConfig
    {
        public static ContainerBuilder InitContainerBuilder()
        {
            Program.Configuration = Program.LoadConfig();
            var containerBuilder = new ContainerBuilder();
            var serviceCollection = new ServiceCollection();
            Program.ConfigureServices(serviceCollection);
            containerBuilder.Populate(serviceCollection);
            Program.RegisterServicesForDependencyInjection(containerBuilder);
            DataAccess.Config.AutofacDiRegistrations.RegisterRepositoriesForDependencyInjection(containerBuilder);
            OverrideDependencyRegistrationsWithTestVersions(containerBuilder);
            return containerBuilder;
        }

        public static AutofacServiceProvider BuildContainer(ContainerBuilder builder)
        {
            return new AutofacServiceProvider(builder.Build());
        }

        private static void OverrideDependencyRegistrationsWithTestVersions(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryMongoContext>().As<IMongoContext>().SingleInstance();
        }
    }
}
