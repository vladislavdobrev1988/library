using System.Linq;
using System.Threading.Tasks;
using Library.Helpers.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private const string HEADER_KEY = "Authorization";
        private const string HEADER_VALUE_PREFIX = "Bearer ";

        private readonly IAccessTokenStore _accessTokenStore;

        public AuthorizationFilter(IAccessTokenStore accessTokenStore)
        {
            _accessTokenStore = accessTokenStore;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (SkipAuthorization(context))
            {
                return;
            }

            var token = GetAccessToken(context.HttpContext.Request.Headers);

            var isValidAccessToken = await _accessTokenStore.IsValidAccessTokenAsync(token);
            if (!isValidAccessToken)
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private bool SkipAuthorization(AuthorizationFilterContext context)
        {
            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            var skip =
                descriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                descriptor.ControllerTypeInfo.IsDefined(typeof(AllowAnonymousAttribute), true);

            return skip;
        }

        private string GetAccessToken(IHeaderDictionary headers)
        {
            if (!headers.ContainsKey(HEADER_KEY) || headers[HEADER_KEY].Count != 1)
            {
                return null;
            }

            var value = headers[HEADER_KEY].Single();

            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith(HEADER_VALUE_PREFIX))
            {
                return null;
            }

            return value.Substring(HEADER_VALUE_PREFIX.Length);
        }
    }
}
