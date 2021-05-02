using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Services.Auths;

namespace UserManagement_Backend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        #region Private Fields
        private readonly IAuthService _authService;
        #endregion

        #region Constructor
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Public Methods
        // Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {
            var response = await _authService.RegisterAsync(userForRegisterDto);

            if (!response.Succeeded)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // Login

        // Refresh Token

        //Revoke Token
        #endregion
    }
}
