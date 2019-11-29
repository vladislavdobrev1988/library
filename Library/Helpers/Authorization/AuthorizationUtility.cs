using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Helpers.Authorization
{
    public class AuthorizationUtility : IAuthorizationUtility
    {
        public bool SkipAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                return false;
            }

            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (descriptor == null)
            {
                return false;
            }

            var skip =
                descriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                descriptor.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), true);

            return skip;
        }
    }
}
