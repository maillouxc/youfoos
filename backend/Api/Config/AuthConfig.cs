using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace YouFoos.Api.Config
{
    /// <summary>
    /// Contains extension methods that can be used to configure the API's authentication.
    /// </summary>
    public static class AuthConfig
    {
        /// <summary>
        /// Configures JWT based authentication for the YouFoos API.
        /// </summary>
        public static void ConfigureJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                var jwtSigningSettings = config.GetSection("JwtSigning").Get<JwtSettings>();
                var jwtSigningSecret = jwtSigningSettings.SigningSecret;
                var jwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningSecret));

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidIssuer = jwtSigningSettings.Issuer,
                    IssuerSigningKey = jwtSigningKey
                };
            });
        }
    }
}
