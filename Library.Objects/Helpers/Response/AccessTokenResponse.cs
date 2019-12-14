namespace Library.Objects.Helpers.Response
{
    public class AccessTokenResponse
    {
        public string AccessToken { get; }

        public AccessTokenResponse(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}
