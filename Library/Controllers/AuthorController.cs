using System.Threading.Tasks;
using Library.Attributes;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("author")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorModel _authorModel;

        public AuthorController(IAuthorModel authorModel)
        {
            _authorModel = authorModel;
        }

        [HttpPost]
        public async Task<ActionResult> Create(AuthorProxy author)
        {
            var id = await _authorModel.CreateAuthorAsync(author);

            return StatusCode(HttpStatusCode.CREATED, new { id });
        }

        [HttpGet]
        [IdRoute]
        public async Task<AuthorProxy> GetById(int id)
        {
            return new AuthorProxy { };
        }
    }
}
