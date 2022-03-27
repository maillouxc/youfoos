using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using YouFoos.Api.Config;
using YouFoos.DataAccess.Entities.Account;

namespace YouFoos.Api.Services.Authentication
{
    /// <summary>
    /// Concrete implementation of <see cref="IJwtMinter"/>.
    /// </summary>
    public class JwtMinter : IJwtMinter
    {
        private readonly JwtSettings _jwtSigningSettings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public JwtMinter(IOptions<JwtSettings> jwtSigningSettings)
        {
            _jwtSigningSettings = jwtSigningSettings.Value;
        }

        /// <summary>
        /// Concrete implementation of <see cref="IJwtMinter.MintJwtForUser(User)"/>.
        /// </summary>
        public string MintJwtForUser(User user)
        {
            var signingCredentials = GetJwtSigningCredentials();

            var token = new JwtSecurityToken(
                issuer: _jwtSigningSettings.Issuer,
                claims: GetClaimsForUser(user),
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSigningSettings.TokenExpirationTimeMinutes)),
                signingCredentials: signingCredentials);

            var signedToken = new JwtSecurityTokenHandler().WriteToken(token);

            return signedToken;
        }

        private SigningCredentials GetJwtSigningCredentials()
        {
            var signingSecretBytes = Encoding.UTF8.GetBytes(_jwtSigningSettings.SigningSecret);
            var signingKey = new SymmetricSecurityKey(signingSecretBytes);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            return signingCredentials;
        }

        private static IEnumerable<Claim> GetClaimsForUser(User user)
        {
            return new[]
            {
                new Claim("Id", user.Id),
                new Claim("Email", user.Email),
                new Claim("Name", user.FirstAndLastName),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };
        }
    }
}
