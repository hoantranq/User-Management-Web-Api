using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using UserManagement_Backend.Services.Users;
using UserManagement_Backend.Helpers;

namespace UserManagement_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        #region Private Fields
        private readonly IUserService _userService;
        #endregion

        #region Constructor
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Private Methods
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _userService.GetAll(User);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            var response = await _userService.GetUserById(userId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Policy = Authorization.ADMIN_ONLY)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserById(string userId)
        {
            var response = await _userService.RemoveUserById(userId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        #endregion
    }
}
