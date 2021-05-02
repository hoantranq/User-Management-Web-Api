using System;
using AutoMapper;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using UserManagement_Backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement_Backend.Models;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.Users
{
    public class UserService : IUserService
    {
        #region Private Fields
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        #endregion

        #region Constructor
        public UserService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        // Get All Users
        public async Task<BaseApiResponse> GetAll(ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(claimsPrincipal);

                var allUserExceptCurrentUser = await _userManager.Users.Where(u => u.Id != currentUser.Id).ToListAsync();

                var users = new List<UserForListingDto>();            

                foreach (var item in allUserExceptCurrentUser)
                {
                    var userRoles = await _userManager.GetRolesAsync(item);

                    var userRolesDto = _mapper.Map<IList<string>>(userRoles);

                    var userForListingDto = _mapper.Map<UserForListingDto>(item);

                    userForListingDto.Roles = userRolesDto;

                    users.Add(userForListingDto);
                }

                return BaseApiResponseHelper.GenerateApiResponse(true, "Get all users", users, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get all users", null, new List<string> { $"{ex.Message}." });
            }
        }

        // Get Specific User
        public async Task<BaseApiResponse> GetUserById(string userId)
        {
            try
            {
                var currentUser = await _userManager.FindByIdAsync(userId);

                var userRoles = await _userManager.GetRolesAsync(currentUser);

                var userRolesDto = _mapper.Map<IList<string>>(userRoles);

                var userForListingDto = _mapper.Map<UserForListingDto>(currentUser);

                userForListingDto.Roles = userRolesDto;

                return BaseApiResponseHelper.GenerateApiResponse(true, "Get user", new List<UserForListingDto> { userForListingDto }, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get user", null, new List<string> { $"{ex.Message}." });
            }
        }

        // Remove User
        public async Task<BaseApiResponse> RemoveUserById(string userId)
        {
            var userToRemove = await _userManager.FindByIdAsync(userId);

            if (userToRemove == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(true, "remove user", null, new List<string> { $"User ID {userId} did not match any users." });
            }

            var result = await _userManager.DeleteAsync(userToRemove);

            if (!result.Succeeded)
            {
                var errs = new List<string>();

                foreach (var err in result.Errors)
                {
                    errs.Add(err.Description);
                }

                return BaseApiResponseHelper.GenerateApiResponse(true, "remove user", null, errs);
            }
            else
            {
                return BaseApiResponseHelper.GenerateApiResponse(true, "Remove user", null, null);
            }
        }
        #endregion
    }
}
