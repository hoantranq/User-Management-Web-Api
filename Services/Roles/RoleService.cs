using System;
using AutoMapper;
using System.Threading.Tasks;
using System.Collections.Generic;
using UserManagement_Backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserManagement_Backend.Models.Responses;
using UserManagement_Backend.Models;
using System.Linq;

namespace UserManagement_Backend.Services.Roles
{
    public class RoleService : IRoleService
    {
        #region Private Fields
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        #endregion

        #region Constructor
        public RoleService(RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        #endregion

        #region Public Methods
        public async Task<BaseApiResponse> GetByRoleId(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "get role", null, new List<string> { $"Role with id {roleId} did not match any roles." });
                }

                var roleDto = _mapper.Map<RoleDto>(role);

                return BaseApiResponseHelper.GenerateApiResponse(true, "Get role", new List<RoleDto> { roleDto }, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get role", null, new List<string> { $"{ex.Message}." });
            }
        }

        public async Task<BaseApiResponse> GetAll()
        {
            try
            {
                var rolesDto = new List<RoleDto>();

                var roles = await _roleManager.Roles.ToListAsync();

                foreach (var item in roles)
                {
                    rolesDto.Add(_mapper.Map<RoleDto>(item));
                }

                return BaseApiResponseHelper.GenerateApiResponse(true, "Get all roles", rolesDto, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get all roles", null, new List<string> { $"{ex.Message}." });
            }
        }

        public async Task<BaseApiResponse> CreateRole(string roleName)
        {
            try
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName.ToLower().Trim()));

                if (!result.Succeeded)
                {
                    var errs = new List<string>();

                    foreach (var item in result.Errors)
                    {
                        errs.Add(item.Description);
                    }
                    return BaseApiResponseHelper.GenerateApiResponse(false, "create a new role", null, errs);
                }
                else
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "Create a new role", null, null);
                }
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "create a new role", null, new List<string> { $"{ex.Message}." });
            }
        }

        public async Task<BaseApiResponse> DeleteRole(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "delete role", null, new List<string> { $"Role with id {roleId} did not match any roles." });
                }

                var result = await _roleManager.DeleteAsync(role);

                if (!result.Succeeded)
                {
                    var errs = new List<string>();

                    foreach (var item in result.Errors)
                    {
                        errs.Add(item.Description);
                    }
                    return BaseApiResponseHelper.GenerateApiResponse(false, "delete role", null, errs);
                }
                else
                {
                    return BaseApiResponseHelper.GenerateApiResponse(true, "Delete role", null, null);
                }
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "delete role", null, new List<string> { $"{ex.Message}." });
            }
        }

        public async Task<BaseApiResponse> ListAllUsersFromRole(string roleId)
        {
            try
            {
                if (roleId == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "list all users from role", null, new List<string> { $"Role Id is null." });
                }

                var role = await _roleManager.FindByIdAsync(roleId);

                if (role == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "list all users from role", null, new List<string> { $"Role with id {roleId} did not match any roles." });
                }

                var users = new List<User>(_userManager.GetUsersInRoleAsync(role.Name).Result);

                var usersToReturn = new List<UserDto>();

                foreach (var item in users)
                {
                    usersToReturn.Add(_mapper.Map<UserDto>(item));
                }

                var data = new RoleForListingUserDto
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    Users = usersToReturn
                };

                return BaseApiResponseHelper.GenerateApiResponse(true, "List all users from role", new List<RoleForListingUserDto> { data }, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "list all users from role", null, new List<string> { $"{ex.Message}." });
            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
