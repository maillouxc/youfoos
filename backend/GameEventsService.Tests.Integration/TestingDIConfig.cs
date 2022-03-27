using Autofac;
using Autofac.Extensions.DependencyInjection;
using DataAccess;
using DataAccess.SharedTestUtils;
using Microsoft.Extensions.DependencyInjection;

namespace GameEventsService.Tests.Integration
{
    public static class TestingDIConfig
    {
        public static AutofacServiceProvider GetTestingDIServiceProvider()
        {
            Program.Configuration = Program.LoadConfig();
            var containerBuilder = new ContainerBuilder();
            var serviceCollection = new ServiceCollection();
            Program.ConfigureServices(serviceCollection);
            containerBuilder.Populate(serviceCollection);
            Program.RegisterServicesForDependencyInjection(containerBuilder);
            DataAccess.Config.AutofacDiRegistrations.RegisterRepositoriesForDependencyInjection(containerBuilder);
            OverrideDependencyRegistrationsWithTestVersions(containerBuilder);
            return new AutofacServiceProvider(containerBuilder.Build());
        }

        private static void OverrideDependencyRegistrationsWithTestVersions(ContainerBuilder builder)
        {
            builder.RegisterType<InMemoryMongoContext>().As<IMongoContext>().SingleInstance();
        }
    }
}
