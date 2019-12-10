using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Library.Filters;
using Library.Helpers;
using Library.Helpers.Authorization;
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
        private const string HEADER_KEY = "Authorization";
        private const string HEADER_VALUE_PREFIX = "Bearer ";

        private Mock<MethodInfo> _actionInfoMock;
        private Mock<TypeInfo> _controllerInfoMock;

        private Mock<HttpContext> _httpContextMock;
        private Mock<HttpRequest> _httpRequestMock;
        private Mock<IHeaderDictionary> _httpHeadersMock;

        private Mock<IAccessTokenStore> _accessTokenStoreMock;

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

            _accessTokenStoreMock = new Mock<IAccessTokenStore>();

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

            _filter = new AuthorizationFilter(_accessTokenStoreMock.Object);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_ActionIsDecoratedWithAllowAnonymous_AuthorizationSkipped()
        {
            _actionInfoMock.Setup(x => x.IsDefined(typeof(AllowAnonymousAttribute), true)).Returns(true);
            
            var context = GetAuthorizationFilterContext();

            await _filter.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_ControllerIsDecoratedWithAllowAnonymous_AuthorizationSkipped()
        {
            _controllerInfoMock.Setup(x => x.IsDefined(typeof(AllowAnonymousAttribute), true)).Returns(true);
            
            var context = GetAuthorizationFilterContext();

            await _filter.OnAuthorizationAsync(context);

            Assert.IsNull(context.Result);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_NoAuthorizationHeader_UnauthorizedReturned()
        {
            var context = GetAuthorizationFilterContext();

            await _filter.OnAuthorizationAsync(context);

            AssertUnauthorizedResult(context.Result);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_InvalidHeaderValue_UnauthorizedReturned()
        {
            var invalidHeaderValue = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HEADER_KEY)).Returns(true);
            _httpHeadersMock.Setup(x => x[HEADER_KEY]).Returns(new StringValues(invalidHeaderValue));

            await _filter.OnAuthorizationAsync(context);

            AssertUnauthorizedResult(context.Result);

            _accessTokenStoreMock.Verify(x => x.IsValidAccessTokenAsync(null), Times.Once);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_InvalidAccessToken_UnauthorizedReturned()
        {
            var token = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HEADER_KEY)).Returns(true);
            _httpHeadersMock.Setup(x => x[HEADER_KEY]).Returns(new StringValues(HEADER_VALUE_PREFIX + token));

            await _filter.OnAuthorizationAsync(context);

            AssertUnauthorizedResult(context.Result);

            _accessTokenStoreMock.Verify(x => x.IsValidAccessTokenAsync(token), Times.Once);
        }

        [TestMethod]
        public async Task OnAuthorizationAsync_ValidAccessToken_ContextResultNull()
        {
            var token = "a";
            var context = GetAuthorizationFilterContext();

            _httpHeadersMock.Setup(x => x.ContainsKey(HEADER_KEY)).Returns(true);
            _httpHeadersMock.Setup(x => x[HEADER_KEY]).Returns(new StringValues(HEADER_VALUE_PREFIX + token));

            _accessTokenStoreMock.Setup(x => x.IsValidAccessTokenAsync(token)).Returns(Task.FromResult(true));

            await _filter.OnAuthorizationAsync(context);
            
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
            Assert.AreEqual((int)HttpStatusCode.Unauthorized, objectResult.StatusCode);
        }
    }
}
