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
        private const string SECRET_KEY = "AccessTokenSecret";

        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly SecurityKey _securityKey;

        public AccessTokenUtility(IConfiguration configuration)
        {
            _configuration = configuration;
            _handler = new JwtSecurityTokenHandler();
            _securityKey = CreateSecurityKey();
        }

        public string CreateAccessToken(DateTime expires, IEnumerable<Claim> claims)
        {
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }

            var descriptor = new SecurityTokenDescriptor
            {
                Expires = expires,
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
            };

            var token = _handler.CreateToken(descriptor);

            return _handler.WriteToken(token);
        }

        private SecurityKey CreateSecurityKey()
        {
            var secret = _configuration.GetValue<string>(SECRET_KEY);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            return securityKey;
        }
    }
}
