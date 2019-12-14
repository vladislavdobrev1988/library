using System.Threading.Tasks;
using Library.Objects.Helpers.Constants;
using Library.Objects.Helpers.Response;
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
        private IAccountModel _accountModel;
        private IUserModel _userModel;

        public AccountController(IAccountModel accountModel, IUserModel userModel)
        {
            _accountModel = accountModel;
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

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<AccessTokenResponse> LogIn(CredentialProxy credentials)
        {
            return await _accountModel.LogIn(credentials);
        }
    }
}
