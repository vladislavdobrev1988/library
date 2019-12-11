using System.Threading.Tasks;
using Library.Objects.Helpers.Constants;
using Library.Objects.Models.Interfaces;
using Library.Objects.Proxies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUserModel _userModel;

        public AccountController(IUserModel userModel)
        {
            _userModel = userModel;
        }

        [HttpPost]
        [Route("signup")]
        [AllowAnonymous]
        public async Task<ActionResult> SignUp(UserProxy user)
        {
            await _userModel.CreateUserAsync(user);

            return StatusCode(HttpStatusCode.CREATED);
        }
    }
}
