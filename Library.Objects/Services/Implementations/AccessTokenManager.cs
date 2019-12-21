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
    public class AccessTokenManager : IAccessTokenManager
    {
        public static class ConfigurationKey
        {
            public const string SECRET = "Security:AccessTokenSecret";
            public const string VALIDITY = "Security:AccessTokenValidityInMinutes";
        }
        
        private readonly JwtSecurityTokenHandler _handler;
        private readonly IConfiguration _configuration;
        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly SecurityKey _securityKey;
        private readonly TokenValidationParameters _validationParameters;

        public AccessTokenManager(JwtSecurityTokenHandler handler, IConfiguration configuration, IDateTimeProvider dateTimeProvider)
        {
            _handler = handler;
            _configuration = configuration;
            _dateTimeProvider = dateTimeProvider;

            _securityKey = CreateSecurityKey();
            _validationParameters = CreateValidationParameters();
        }

        public string CreateAccessToken(IEnumerable<Claim> claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var validity = double.Parse(_configuration[ConfigurationKey.VALIDITY]);

            var expires = _dateTimeProvider.UtcNow.AddMinutes(validity);

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
                return CreateUnauthorized();
            }

            ClaimsPrincipal principal;

            try
            {
                principal = _handler.ValidateToken(token, _validationParameters, out SecurityToken securityToken);
            }
            catch (Exception)
            {
                return CreateUnauthorized();
            }

            return principal.Identity as ClaimsIdentity;
        }

        private SecurityKey CreateSecurityKey()
        {
            var secret = _configuration[ConfigurationKey.SECRET];

            var key = Encoding.UTF8.GetBytes(secret);

            return new SymmetricSecurityKey(key);
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

        private ClaimsIdentity CreateUnauthorized()
        {
            return new ClaimsIdentity();
        }
    }
}
