using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Library.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
            {
                return;
            }

            context.Result = new UnauthorizedResult();
        }

        private bool SkipAuthorization(AuthorizationFilterContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var skip =
                descriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                descriptor.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), true);

            return skip;
        }
    }
}
