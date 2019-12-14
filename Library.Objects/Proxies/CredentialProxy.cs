using Library.Objects.Proxies.Base;

namespace Library.Objects.Proxies
{
    public class CredentialProxy : BaseProxy
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
