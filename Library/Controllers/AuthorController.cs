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
        public async Task<ActionResult> CreateAsync(AuthorProxy author)
        {
            var response = await _authorModel.CreateAuthorAsync(author);

            return StatusCode(HttpStatusCode.CREATED, response);
        }

        [HttpGet]
        [IdRoute]
        public async Task<AuthorProxy> GetByIdAsync(int id)
        {
            return await _authorModel.GetAuthorByIdAsync(id);
        }
    }
}
