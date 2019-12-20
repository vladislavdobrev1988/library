using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using Library.Filters;
using Library.Objects.Helpers.Constants;
using Library.Objects.Helpers.Response;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.WebApiTests
{
    [TestClass]
    public class AuthorizationFilterTests
    {
        private Mock<MethodInfo> _actionInfoMock;
        private Mock<TypeInfo> _controllerInfoMock;

        private Mock<HttpContext> _httpContextMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IHeaderDictionary> _httpHeadersMock;
        private Mock<ClaimsIdentity> _claimsIdentityMock;

        private Mock<IAccessTokenManager> _accessTokenManagerMock;

        private ActionContext _actionContext;

        private AuthorizationFilter _filter;

        [TestInitialize]
        public void Init()
        {
            _actionInfoMock = new Mock<MethodInfo>();
            _controllerInfoMock = new Mock<TypeInfo>();

            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();
            _httpHeadersMock = new Mock<IHeaderDictionary>();
            _claimsIdentityMock = new Mock<ClaimsIdentity>();

            _accessTokenManagerMock = new Mock<IAccessTokenManager>();

            _httpRequestMock.Setup(x => x.Headers).Returns(_httpHeadersMock.Object);
            _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);

            _actionContext = new ActionContext
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    MethodInfo = _actionInfoMock.Object,
                    ControllerTypeInfo = _controllerInfoMock.Object
                },
                HttpContext = _httpContextMock.Object,
                RouteData = new RouteData()
            };

            _filter = new AuthorizationFilter(_accessTokenManagerMock.Object);
        }

        [TestMethod]
        public void OnAuthorization_ActionIsDecoratedWithAllowAnonymous_AuthorizationSkipped()
        {
            _actionInfoMock.Setup(x => x.IsDefined(typeof(AllowAnonymousAttribute), true)).Returns(true);
            
            var context = GetAuthorizationFilterContext();

            _filter.OnAuthorization(context);

            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public void OnAuthorization_ControllerIsDecoratedWithAllowAnonymous_AuthorizationSkipped()
        {
            _controllerInfoMock.Setup(x => x.IsDefined(typeof(AllowAnonymousAttribute), true)).Returns(true);
            
            var context = GetAuthorizationFilterContext();

            _filter.OnAuthorization(context);

            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public void OnAuthorization_NoAuthorizationHeader_UnauthorizedReturned()
        {
            var context = GetAuthorizationFilterContext();
            
            _accessTokenManagerMock.Setup(x => x.GetIdentity(null)).Returns(_claimsIdentityMock.Object);

            _filter.OnAuthorization(context);

            AssertUnauthorizedResult(context.Result);
        }

        [TestMethod]
        public void OnAuthorization_InvalidHeaderValue_UnauthorizedReturned()
        {
            var invalidHeaderValue = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HttpHeader.AUTHORIZATION)).Returns(true);
            _httpHeadersMock.Setup(x => x[HttpHeader.AUTHORIZATION]).Returns(new StringValues(invalidHeaderValue));

            _accessTokenManagerMock.Setup(x => x.GetIdentity(null)).Returns(_claimsIdentityMock.Object);

            _filter.OnAuthorization(context);

            AssertUnauthorizedResult(context.Result);
        }

        [TestMethod]
        public void OnAuthorization_InvalidAccessToken_UnauthorizedReturned()
        {
            var token = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HttpHeader.AUTHORIZATION)).Returns(true);
            _httpHeadersMock.Setup(x => x[HttpHeader.AUTHORIZATION]).Returns(new StringValues(GetHeaderValue(token)));

            _accessTokenManagerMock.Setup(x => x.GetIdentity(token)).Returns(_claimsIdentityMock.Object);

            _filter.OnAuthorization(context);

            AssertUnauthorizedResult(context.Result);
        }

        [TestMethod]
        public void OnAuthorization_ValidAccessToken_ContextResultNull()
        {
            var token = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HttpHeader.AUTHORIZATION)).Returns(true);
            _httpHeadersMock.Setup(x => x[HttpHeader.AUTHORIZATION]).Returns(new StringValues(GetHeaderValue(token)));

            _claimsIdentityMock.Setup(x => x.IsAuthenticated).Returns(true);

            _accessTokenManagerMock.Setup(x => x.GetIdentity(token)).Returns(_claimsIdentityMock.Object);

            _filter.OnAuthorization(context);
            
            Assert.IsNull(context.Result);
        }

        private AuthorizationFilterContext GetAuthorizationFilterContext()
        {
            return new AuthorizationFilterContext(_actionContext, new List<IFilterMetadata>(0));
        }

        private void AssertUnauthorizedResult(IActionResult result)
        {
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            var objectResult = result as ObjectResult;

            Assert.IsInstanceOfType(objectResult.Value, typeof(MessageResponse));
            var messageResponse = objectResult.Value as MessageResponse;

            Assert.AreEqual(AuthorizationFilter.UNAUTHORIZED_MESSAGE, messageResponse.Message);
            Assert.AreEqual(HttpStatusCode.UNAUTHORIZED, objectResult.StatusCode);
        }

        private static string GetHeaderValue(string token)
        {
            return string.Join(" ", HttpAuthenticationScheme.BEARER, token);
        }
    }
}
