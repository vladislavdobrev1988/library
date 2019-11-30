using System.Threading.Tasks;
using Library.Attributes;
using Library.Objects.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("author")]
    [ApiController]
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
