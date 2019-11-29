using Microsoft.AspNetCore.Mvc.Filters;

namespace Library.Helpers.Authorization
{
    public interface IAuthorizationUtility
    {
        bool SkipAuthorization(AuthorizationFilterContext context);
    }
}
