using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement_Backend.DTOs;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var response = await _roleService.GetByRoleId(id);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

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

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var response = await _roleService.DeleteRole(id);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        #endregion
    }
}
