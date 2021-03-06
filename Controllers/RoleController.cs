using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Helpers;
using Microsoft.AspNetCore.Authorization;
using UserManagement_Backend.Services.Roles;

namespace UserManagement_Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/role")]
    public class RoleController : ControllerBase
    {
        #region Private Fields
        private readonly IRoleService _roleService;
        #endregion

        #region Constructor
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        #endregion

        #region Public Methods
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _roleService.GetAll();

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetRoleById(string roleId)
        {
            var response = await _roleService.GetByRoleId(roleId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Policy = Authorization.ADMIN_ONLY)]
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] RoleForCreateDto roleForCreateDto)
        {
            var response = await _roleService.CreateRole(roleForCreateDto.Name);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Policy = Authorization.ADMIN_ONLY)]
        [HttpDelete("delete/{roleId}")]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var response = await _roleService.DeleteRole(roleId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("users-from-role/{roleId}")]
        public async Task<IActionResult> GetUsersFromRole(string roleId)
        {
            var response = await _roleService.ListAllUsersFromRole(roleId);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        #endregion
    }
}
