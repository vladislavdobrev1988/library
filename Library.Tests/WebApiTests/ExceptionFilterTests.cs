using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Library.Filters;
using Library.Helpers;
using Library.Objects.Exceptions;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Library.Tests.WebApiTests
{
    [TestClass]
    public class ExceptionFilterTests
    {
        private Mock<IExceptionLogger> _exceptionLoggerMock;

        private ExceptionFilter _filter;

        [TestInitialize]
        public void Init()
        {
            _exceptionLoggerMock = new Mock<IExceptionLogger>();
            _filter = new ExceptionFilter(_exceptionLoggerMock.Object);
        }

        [TestMethod]
        public async Task OnExceptionAsync_HttpResponseException_ReturnsExpectedResult()
        {
            var exception = new HttpResponseException(HttpStatusCode.NotFound, "some");

            var context = GetExceptionContext(exception);

            await _filter.OnExceptionAsync(context);

            var objectResult = context.Result as ObjectResult;
            Assert.IsNotNull(objectResult);

            var exceptionResponse = objectResult.Value as ExceptionResponse;
            Assert.IsNotNull(exceptionResponse);

            Assert.AreEqual(exception.Message, exceptionResponse.Message);

            Assert.AreEqual((int)exception.StatusCode, objectResult.StatusCode);

            Assert.IsTrue(context.ExceptionHandled);

            _exceptionLoggerMock.Verify(x => x.LogAsync(exception), Times.Once);
        }

        [TestMethod]
        public async Task OnExceptionAsync_ArgumentNullException_ReturnsExpectedResult()
        {
            var exception = new ArgumentNullException("param1");

            var context = GetExceptionContext(exception);

            await _filter.OnExceptionAsync(context);

            var objectResult = context.Result as ObjectResult;
            Assert.IsNotNull(objectResult);

            var exceptionResponse = objectResult.Value as ExceptionResponse;
            Assert.IsNotNull(exceptionResponse);

            Assert.AreEqual(ExceptionFilter.INTERNAL_SERVER_ERROR_MESSAGE, exceptionResponse.Message);

            Assert.AreEqual((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);

            Assert.IsTrue(context.ExceptionHandled);

            _exceptionLoggerMock.Verify(x => x.LogAsync(exception), Times.Once);
        }

        private ExceptionContext GetExceptionContext(Exception exception)
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());

            return new ExceptionContext(actionContext, new List<IFilterMetadata>(0))
            {
                Exception = exception
            };
        }
    }
}
