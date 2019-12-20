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
        private const string SECRET_KEY = "Security:AccessTokenSecret";
        private const string VALIDITY_KEY = "Security:AccessTokenValidityInMinutes";

        private readonly JwtSecurityTokenHandler _handler;
        private readonly IConfiguration _configuration;
        private readonly SecurityKey _securityKey;
        private readonly TokenValidationParameters _validationParameters;

        public AccessTokenManager(JwtSecurityTokenHandler handler, IConfiguration configuration)
        {
            _handler = handler;
            _configuration = configuration;

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

        private ClaimsIdentity CreateUnauthorized()
        {
            return new ClaimsIdentity();
        }
    }
}
