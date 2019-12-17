using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Library.Objects.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Library.Objects.Services.Implementations
{
    public class AccessTokenUtility : IAccessTokenUtility
    {
        private const string SECRET_KEY = "Security:AccessTokenSecret";
        private const string VALIDITY_KEY = "Security:AccessTokenValidityInMinutes";

        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly SecurityKey _securityKey;
        private readonly TokenValidationParameters _validationParameters;

        public AccessTokenUtility(IConfiguration configuration)
        {
            _configuration = configuration;
            _handler = new JwtSecurityTokenHandler();

            _securityKey = CreateSecurityKey();
            _validationParameters = CreateValidationParameters();
        }

        public string CreateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var validity = _configuration.GetValue<double>(VALIDITY_KEY);

            var expires = DateTime.UtcNow.AddMinutes(validity);

            var descriptor = new SecurityTokenDescriptor
            {
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = _handler.CreateJwtSecurityToken(descriptor);

            return _handler.WriteToken(token);
        }

        public ClaimsIdentity GetIdentity(string token)
        {
            if (!_handler.CanReadToken(token))
            {
                return CreateUnauthenticated();
            }

            ClaimsPrincipal principal;

            try
            {
                principal = _handler.ValidateToken(token, _validationParameters, out SecurityToken securityToken);
            }
            catch (SecurityTokenValidationException)
            {
                return CreateUnauthenticated();
            }

            return principal.Identity as ClaimsIdentity;
        }

        private SecurityKey CreateSecurityKey()
        {
            var secret = _configuration.GetValue<string>(SECRET_KEY);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            return securityKey;
        }

        private TokenValidationParameters CreateValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = _securityKey,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuer = false,
                ValidateAudience = false
            };
        }

        private ClaimsIdentity CreateUnauthenticated()
        {
            return new ClaimsIdentity();
        }
    }
}
