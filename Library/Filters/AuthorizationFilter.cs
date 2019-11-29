using Library.Helpers.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Library.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IAuthorizationUtility _utility;

        public AuthorizationFilter(IAuthorizationUtility utility)
        {
            _utility = utility;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (_utility.SkipAuthorization(context))
            {
                return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}
