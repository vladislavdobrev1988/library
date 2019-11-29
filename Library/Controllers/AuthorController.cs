using Library.Attributes;
using Library.Objects.Proxies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.Controllers
{
    [Route("author")]
    [ApiController]
    [AllowAnonymous]
    public class AuthorController : ControllerBase
    {
        [HttpGet]
        [IdRoute]
        public async Task<AuthorProxy> GetById(int id)
        {
            return new AuthorProxy { };
        }
    }
}
