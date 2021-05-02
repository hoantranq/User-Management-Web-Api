using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement_Backend.Context;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Helpers;
using UserManagement_Backend.Models;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.UserRoles
{
    public class UserRoleService : IUserRoleService
    {
        #region Private Fields
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public UserRoleService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IMapper mapper, SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
        }
        #endregion

        #region Public Methods
        public async Task<BaseApiResponse> GetUserRoles(string userId)
        {
            try
            {
                if (userId == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "get roles for user", null, new List<string> { $"User ID is null." });
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "get roles for user", null, 
                        new List<string> { $"User ID {userId} did not match any users." });
                }

                var listUserRolesDto = new List<UserRolesDto>();

                foreach (var role in await _roleManager.Roles.ToListAsync())
                {
                    var userRolesDto = new UserRolesDto
                    {
                        RoleName = role.Name
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesDto.Selected = true;
                    }
                    else
                    {
                        userRolesDto.Selected = false;
                    }

                    listUserRolesDto.Add(userRolesDto);
                }

                var data = new UserRolesForEditDto
                { 
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    UserRoles = listUserRolesDto
                };

                return BaseApiResponseHelper.GenerateApiResponse(true, "Get roles for user", new List<UserRolesForEditDto> { data }, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get roles for user", null, new List<string> { $"{ex.Message}." });
            }
        }

        public async Task<BaseApiResponse> UpdateUserRoles(UserRolesForEditDto userRolesForEditDto, ClaimsPrincipal claimsPrincipal)
        {
            try
            {
                if (userRolesForEditDto.UserId == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "update roles for user", null, new List<string> { $"User ID is null." });
                }

                var user = await _userManager.FindByIdAsync(userRolesForEditDto.UserId);

                if (user == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "update roles for user", null, 
                        new List<string> { $"User ID {userRolesForEditDto.UserId} did not match any users." });
                }

                var roles = await _userManager.GetRolesAsync(user);

                var result = await _userManager.RemoveFromRolesAsync(user, roles);

                // In case we forgot to select any roles for user, we will add this user to role user by default.
                var isUserInAnyRoles = userRolesForEditDto.UserRoles.Any(x => x.Selected == true);

                if (!isUserInAnyRoles)
                {
                    result = await _userManager.AddToRolesAsync(user, new List<string> { Authorization.DEFAULT_ROLE.ToString() });
                }
                else
                {
                    result = await _userManager.AddToRolesAsync(user, userRolesForEditDto.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));
                }

                var currentUser = await _userManager.GetUserAsync(claimsPrincipal);

                await _signInManager.RefreshSignInAsync(currentUser);

                await ApplicationDbInitializer.SeedAdministratorUser(_userManager, _roleManager);

                return BaseApiResponseHelper.GenerateApiResponse(true, "Update roles for user", null, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "update roles for user", null, new List<string> { $"{ex.Message}." });
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
