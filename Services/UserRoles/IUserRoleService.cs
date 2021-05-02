using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.UserRoles
{
    public interface IUserRoleService
    {
        Task<BaseApiResponse> GetUserRoles(string userId);

        Task<BaseApiResponse> UpdateUserRoles(UserRolesForEditDto userRolesForEditDto, ClaimsPrincipal claimsPrincipal);
    }
}
