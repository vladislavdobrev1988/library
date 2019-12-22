using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Library.Objects.Services.Implementations;
using Library.Objects.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.ServiceTests
{
    [TestClass]
    public class AccessTokenManagerTests
    {
        private const double VALIDITY_IN_MINUTES = 20;
        private const string SECRET = "KA0234JVASFMBEH34BCV";

        private delegate void ValidateCallback(string token, TokenValidationParameters validationParameters, out SecurityToken validatedToken);

        private readonly Mock<JwtSecurityTokenHandler> _jwtSecurityTokenHandlerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        private readonly AccessTokenManager _manager;

        public AccessTokenManagerTests()
        {
            _jwtSecurityTokenHandlerMock = new Mock<JwtSecurityTokenHandler>();
            _configurationMock = new Mock<IConfiguration>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();
            
            _configurationMock
                .Setup(x => x[AccessTokenManager.ConfigurationKey.VALIDITY])
                .Returns(VALIDITY_IN_MINUTES.ToString());

            _configurationMock
                .Setup(x => x[AccessTokenManager.ConfigurationKey.SECRET])
                .Returns(SECRET);

            _manager = new AccessTokenManager(_jwtSecurityTokenHandlerMock.Object, _configurationMock.Object, _dateTimeProviderMock.Object);
        }

        [TestMethod]
        public void CreateAccessToken_NullClaims_ThrowsException()
        {
            Claim[] claims = null;

            var ex = Assert.ThrowsException<ArgumentNullException>(() => _manager.CreateAccessToken(claims));
            Assert.AreEqual(nameof(claims), ex.ParamName);
        }

        [TestMethod]
        public void CreateAccessToken_ValidInput_WorksAsExpected()
        {
            const string EXPECTED_EMAIL = "a@b.com";
            const string EXPECTED_TOKEN = "someheader.somepayload.somesignature";

            var utcNow = DateTime.UtcNow;            
            _dateTimeProviderMock.Setup(x => x.UtcNow).Returns(utcNow);

            var jwtSecurityToken = new JwtSecurityToken();
            SecurityTokenDescriptor passedDescriptor = null;

            _jwtSecurityTokenHandlerMock
                .Setup(x => x.CreateJwtSecurityToken(It.IsAny<SecurityTokenDescriptor>()))
                .Callback<SecurityTokenDescriptor>(x => passedDescriptor = x)
                .Returns(jwtSecurityToken);

            _jwtSecurityTokenHandlerMock
                .Setup(x => x.WriteToken(jwtSecurityToken))
                .Returns(EXPECTED_TOKEN);

            var token = _manager.CreateAccessToken(new[] { new Claim(ClaimTypes.Email, EXPECTED_EMAIL) });

            Assert.AreEqual(EXPECTED_TOKEN, token);
            Assert.IsNotNull(passedDescriptor);

            var expectedExpires = utcNow.AddMinutes(VALIDITY_IN_MINUTES);
            Assert.AreEqual(expectedExpires, passedDescriptor.Expires);

            Assert.IsNotNull(passedDescriptor.Subject);
            Assert.IsNotNull(passedDescriptor.Subject.Claims);
            Assert.AreEqual(1, passedDescriptor.Subject.Claims.Count());

            var claim = passedDescriptor.Subject.Claims.Single();

            Assert.AreEqual(ClaimTypes.Email, claim.Type);
            Assert.AreEqual(EXPECTED_EMAIL, claim.Value);

            Assert.IsNotNull(passedDescriptor.SigningCredentials);
            Assert.AreEqual(passedDescriptor.SigningCredentials.Algorithm, SecurityAlgorithms.HmacSha256);

            AssertSecurityKey(passedDescriptor.SigningCredentials.Key);
        }

        [TestMethod]
        public void GetIdentity_MalformedToken_ReturnsUnauthorized()
        {
            const string TOKEN = "some";

            _jwtSecurityTokenHandlerMock.Setup(x => x.CanReadToken(TOKEN)).Returns(false);

            var result = _manager.GetIdentity(TOKEN);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAuthenticated);

            SecurityToken token;
            _jwtSecurityTokenHandlerMock.Verify(x => x.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>(), out token), Times.Never);
        }

        [TestMethod]
        public void GetIdentity_InvalidToken_ReturnsUnauthorized()
        {
            const string TOKEN = "some";

            _jwtSecurityTokenHandlerMock.Setup(x => x.CanReadToken(TOKEN)).Returns(true);
            
            TokenValidationParameters passedParameters = null;
            var callback = new ValidateCallback((string t, TokenValidationParameters p, out SecurityToken st) =>
            {
                st = null;
                passedParameters = p;
            });

            SecurityToken token;
            _jwtSecurityTokenHandlerMock
                .Setup(h => h.ValidateToken(TOKEN, It.IsAny<TokenValidationParameters>(), out token))
                .Callback(callback)
                .Throws(new Exception());

            var result = _manager.GetIdentity(TOKEN);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsAuthenticated);

            Assert.IsNotNull(passedParameters);
            Assert.IsTrue(passedParameters.RequireExpirationTime);
            Assert.IsTrue(passedParameters.ValidateLifetime);
            Assert.IsFalse(passedParameters.ValidateIssuer);
            Assert.IsFalse(passedParameters.ValidateAudience);

            AssertSecurityKey(passedParameters.IssuerSigningKey);
        }

        [TestMethod]
        public void GetIdentity_ValidToken_WorksAsExpected()
        {
            const string TOKEN = "some";

            _jwtSecurityTokenHandlerMock.Setup(x => x.CanReadToken(TOKEN)).Returns(true);

            var principal = new ClaimsPrincipal(new ClaimsIdentity());

            TokenValidationParameters passedParameters = null;
            var callback = new ValidateCallback((string t, TokenValidationParameters p, out SecurityToken st) =>
            {
                st = null;
                passedParameters = p;
            });

            SecurityToken token;
            _jwtSecurityTokenHandlerMock
                .Setup(h => h.ValidateToken(TOKEN, It.IsAny<TokenValidationParameters>(), out token))
                .Callback(callback)
                .Returns(principal);

            var result = _manager.GetIdentity(TOKEN);

            Assert.IsNotNull(result);
            Assert.AreSame(principal.Identity, result);

            Assert.IsNotNull(passedParameters);
            Assert.IsTrue(passedParameters.RequireExpirationTime);
            Assert.IsTrue(passedParameters.ValidateLifetime);
            Assert.IsFalse(passedParameters.ValidateIssuer);
            Assert.IsFalse(passedParameters.ValidateAudience);

            AssertSecurityKey(passedParameters.IssuerSigningKey);
        }

        private void AssertSecurityKey(SecurityKey securityKey)
        {
            Assert.IsNotNull(securityKey);
            Assert.IsInstanceOfType(securityKey, typeof(SymmetricSecurityKey));

            var symmetricSecurityKey = securityKey as SymmetricSecurityKey;
            var secret = Encoding.UTF8.GetString(symmetricSecurityKey.Key);

            Assert.AreEqual(SECRET, secret);
        }
    }
}
