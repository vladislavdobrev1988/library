using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Library.Objects.Services.Implementations;
using Library.Objects.Services.Interfaces;
using Microsoft.Extensions.Configuration;
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
            //var claim = new Claim(ClaimTypes.Email, "a@b.com");

            //var token = "GFQA1WRE.O6ITNA8.SDE6VW8E";

            //var jwtSecurityToken = new JwtSecurityToken();

            //SecurityTokenDescriptor descriptor = null;

            //_jwtSecurityTokenHandlerMock
            //    .Setup(x => x.CreateJwtSecurityToken(It.IsAny<SecurityTokenDescriptor>()))
            //    .Returns(jwtSecurityToken)
            //    .Callback<SecurityTokenDescriptor>(x => descriptor = x);

            //_jwtSecurityTokenHandlerMock
            //    .Setup(x => x.WriteToken(jwtSecurityToken))
            //    .Returns(token);

            //var result = _manager.CreateAccessToken(new[] { claim });

            //Assert.AreEqual(token, result);

            //Assert.IsNotNull(descriptor);
        }

        [TestMethod]
        public void GetIdentity_WorksAsExpected()
        {

        }
    }
}
