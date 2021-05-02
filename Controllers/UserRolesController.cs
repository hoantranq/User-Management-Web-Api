using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using UserManagement_Backend.Services.UserRoles;

namespace UserManagement_Backend.Controllers
{
    [Authorize(Policy = Authorization.ADMIN_ONLY)]
    [Authorize]
    [ApiController]
    [Route("api/user-roles")]
    public class UserRolesController : ControllerBase
    {
        #region Private Fields
        private readonly IUserRoleService _userRoleService;
        #endregion

        #region Constructor
        public UserRolesController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }
        #endregion

        #region Public Methods
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            var response = await _userRoleService.GetUserRoles(userId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UserRolesForEditDto userRolesForEditDto)
        {
            var response = await _userRoleService.UpdateUserRoles(userRolesForEditDto, User);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        #endregion
    }
}
