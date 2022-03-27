using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// This class is where the swagger docs for the API are configured.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class SwaggerConfig
    {
        /// <summary>
        /// Configures the API to generate swagger JSON output that can later be used by Swagger UI.
        /// </summary>
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGenNewtonsoftSupport();

            return services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "YouFoos API",
                        Description = "Official documentation for developers using the YouFoos API",
                        Contact = new OpenApiContact()
                        {
                            Name = "YouFoos Support",
                            Email = "mailloux.cl@gmail.com",
                            Url = new Uri("http://www.github.com/maillouxc")
                        }
                    }
                );

                config.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your non-expired auth token. Auth tokens can be obtained by calling the login route."
                });

                config.OperationFilter<SwaggerNoAuthEndpointsOperationFilter>();
                config.DescribeAllEnumsAsStrings();

                // Find all the XML comments used to generate info in Swagger UI.
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml");
                foreach (var xmlFile in xmlFiles)
                {
                    config.IncludeXmlComments(xmlFile);
                }
            });
        }

        /// <summary>
        /// Configures the YouFoos API custom Swagger UI settings.
        /// </summary>
        public static void ConfigureCustomSwaggerUi(this IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "YouFoos API v1");
                options.InjectStylesheet("/swagger-ui/custom.css");
                options.InjectJavascript("/swagger-ui/custom.js");
                options.DocumentTitle = "YouFoos API";
            });
        }
    }
}
