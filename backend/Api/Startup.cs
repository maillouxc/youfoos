using System.Diagnostics.CodeAnalysis;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YouFoos.Api.Config;
using YouFoos.Api.Middleware;
using YouFoos.DataAccess.Config;

namespace YouFoos.Api
{
    /// <summary>
    /// The standard startup class that all .NET Web APIs have.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        /// <summary>
        /// The configuration object that holds various API config options.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime - use it to configure the HTTP request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment _)
        {
            app.UseRouting();
            app.ConfigureCustomSwaggerUi();
            app.UseCors(policyName: "AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<SerilogMiddleware>();
            app.UseEndpoints(endpoints => 
            { 
                endpoints.MapControllers(); 
            });
        }

        /// <summary>
        /// This method gets called by the runtime - use it to configure the service collection.
        /// </summary>
        public void ConfigureServices(IServiceCollection services)
        {
            LoadOptionsForDiFromAppSettingsJson(services);

            services.ConfigureCors();
            services.ConfigureJwtAuthentication(Configuration);
            services.ConfigureSwagger().AddSwaggerGenNewtonsoftSupport();
            services.AddControllersWithViews().AddNewtonsoftJson();
            services.AddControllers(options =>
            {
                // TODO disables a breaking change from .NET core 2.2 -> 3.0 upgrade - fix when possible
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddNewtonsoftJson();
        }

        /// <summary>
        /// This is where we register things directory with our DI provider, in this case Autofac.
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            DiRegistrations.RegisterStuffForDependencyInjection(builder);
        }

        private void LoadOptionsForDiFromAppSettingsJson(IServiceCollection services)
        {
            services.Configure<MongoSettings>(Configuration.GetSection("MongoDB"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<JwtSettings>(Configuration.GetSection("JwtSigning"));
        }
    }
}
