using Library.Helpers;
using Library.Objects.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var ex = context.Exception as HttpResponseException;
            if (ex != null)
            {
                var response = new ExceptionResponse
                {
                    Message = ex.Message
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int)ex.StatusCode
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
