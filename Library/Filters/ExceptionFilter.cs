using System.Net;
using System.Threading.Tasks;
using Library.Helpers;
using Library.Objects.Exceptions;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IExceptionLogger _exceptionLogger;

        private const string INTERNAL_SERVER_ERROR_MESSAGE = "Internal Server Error";

        public ExceptionFilter(IExceptionLogger exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string message;
            HttpStatusCode statusCode;

            if (context.Exception is HttpResponseException ex)
            {
                message = ex.Message;
                statusCode = ex.StatusCode;
            }
            else
            {
                message = INTERNAL_SERVER_ERROR_MESSAGE;
                statusCode = HttpStatusCode.InternalServerError;
            }

            var response = new ExceptionResponse(message);

            context.Result = new ObjectResult(response)
            {
                StatusCode = (int)statusCode
            };

            context.ExceptionHandled = true;

            await _exceptionLogger.LogAsync(context.Exception);
        }
    }
}
