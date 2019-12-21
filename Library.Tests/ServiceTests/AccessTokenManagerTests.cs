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

        private Mock<JwtSecurityTokenHandler> _jwtSecurityTokenHandlerMock;
        private Mock<IConfiguration> _configurationMock;
        private Mock<IDateTimeProvider> _dateTimeProviderMock;

        private AccessTokenManager _manager;

        [TestInitialize]
        public void Init()
        {
            _jwtSecurityTokenHandlerMock = new Mock<JwtSecurityTokenHandler>();
            _configurationMock = new Mock<IConfiguration>();
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            _configurationMock.Setup(x => x[AccessTokenManager.ConfigurationKey.VALIDITY]).Returns(VALIDITY_IN_MINUTES.ToString());
            _configurationMock.Setup(x => x[AccessTokenManager.ConfigurationKey.SECRET]).Returns(SECRET);

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
                .Returns(jwtSecurityToken)
                .Callback<SecurityTokenDescriptor>(x => passedDescriptor = x);

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

            Assert.IsNotNull(passedDescriptor.SigningCredentials.Key);
            Assert.IsInstanceOfType(passedDescriptor.SigningCredentials.Key, typeof(SymmetricSecurityKey));

            var securityKey = passedDescriptor.SigningCredentials.Key as SymmetricSecurityKey;
            var secret = Encoding.UTF8.GetString(securityKey.Key);
            Assert.AreEqual(SECRET, secret);
        }

        [TestMethod]
        public void GetIdentity_NullToken_ReturnsUnauthorized()
        {

        }

        [TestMethod]
        public void GetIdentity_WhitespaceToken_ReturnsUnauthorized()
        {

        }

        [TestMethod]
        public void GetIdentity_InvalidToken_ReturnsUnauthorized()
        {

        }

        [TestMethod]
        public void GetIdentity_ValidToken_WorksAsExpected()
        {

        }
    }
}
