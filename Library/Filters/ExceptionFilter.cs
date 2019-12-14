using System.Threading.Tasks;
using Library.Objects.Exceptions;
using Library.Objects.Helpers.Constants;
using Library.Objects.Helpers.Response;
using Library.Objects.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        private readonly IExceptionLogger _exceptionLogger;

        public const string INTERNAL_SERVER_ERROR_MESSAGE = "Internal Server Error";

        public ExceptionFilter(IExceptionLogger exceptionLogger)
        {
            _exceptionLogger = exceptionLogger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string message;
            int statusCode;

            if (context.Exception is HttpResponseException ex)
            {
                message = ex.Message;
                statusCode = ex.StatusCode;
            }
            else
            {
                message = INTERNAL_SERVER_ERROR_MESSAGE;
                statusCode = HttpStatusCode.INTERNAL_SERVER_ERROR;
            }

            context.Result = new ObjectResult(new MessageResponse(message))
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;

            await _exceptionLogger.LogAsync(context.Exception);
        }
    }
}
