using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using UserManagement_Backend.Models;
using UserManagement_Backend.Models.Responses;
using UserManagement_Backend.Helpers;
using AutoMapper;
using UserManagement_Backend.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserManagement_Backend.DTOs;
using System.Collections.Generic;

namespace UserManagement_Backend.Services.Auths
{
    public class AuthService : IAuthService
    {
        #region Private Fields
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        #endregion

        #region Constructor
        public AuthService(
            UserManager<User> userManager,
            IOptions<JWT> jwt,
            IMapper mapper,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _jwt = jwt.Value;
        }
        #endregion

        #region Public Methods
        // Register
        public async Task<BaseApiResponse> RegisterAsync(UserForRegisterDto userForRegisterDto)
        {
            var newUser = _mapper.Map<User>(userForRegisterDto);

            var userFromDb = await _userManager.FindByEmailAsync(userForRegisterDto.Email);

            if (userFromDb != null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "register new user", null, new List<string> { $"Email {newUser.Email} is already registered" });
            }

            var result = await _userManager.CreateAsync(newUser, userForRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Authorization.DEFAULT_ROLE.ToString());
            }

            return BaseApiResponseHelper.GenerateApiResponse(true, "Register new user", null, null);
        }

        // Login
        //public async Task<BaseApiResponse> LoginAsync()
        //{

        //}

        // Refresh Token
        //public async Task<BaseApiResponse> RefreshTokenAsync()
        //{

        //}

        // Revoke Token
        //public async Task<BaseApiResponse> RevokeTokenAsync()
        //{

        //}

        #endregion

        #region Private Methods
        //private async Task<JwtSecurityToken> CreateJwtToken(User user)
        //{

        //}


        #endregion
    }
}
