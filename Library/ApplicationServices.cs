using Library.Helpers.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Library
{
    public static class ApplicationServices
    {
        public static void Register(IServiceCollection services)
        {
            services.AddSingleton<IAccessTokenStore, AccessTokenStore>();
        }
    }
}
