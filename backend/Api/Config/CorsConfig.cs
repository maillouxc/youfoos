using Microsoft.Extensions.DependencyInjection;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// This class contains the code needed to configure CORS settings for the application.
    /// </summary>
    public static class CorsConfig
    {
        /// <summary>
        /// Configures the appropriate CORS settings for the YouFoos API.
        /// </summary>
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
            });
        }
    }
}
