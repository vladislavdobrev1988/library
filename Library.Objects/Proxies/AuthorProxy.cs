using System;
using Library.Objects.Proxies.Base;

namespace Library.Objects.Proxies
{
    public class AuthorProxy : BaseProxy
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfDeath { get; set; }
    }
}
