using System.Threading.Tasks;

namespace Library.Helpers.Authorization
{
    public class AccessTokenStore : IAccessTokenStore
    {
        private const string DUMMY_ACCESS_TOKEN = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6InZsYWRpc2xhdmRvYnJldjE5ODhAZ21haWwuY29tIn0.yyWxX09_XFaDahOJSoOEjpcj6norQXnw_lpSu3kTmqQ";

        public async Task<bool> IsValidAccessTokenAsync(string accessToken)
        {
            return accessToken == DUMMY_ACCESS_TOKEN;
        }
    }
}
