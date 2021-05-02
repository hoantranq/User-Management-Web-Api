using System.Threading.Tasks;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.Roles
{
    public interface IRoleService
    {
        Task<BaseApiResponse> GetByRoleId(string roleId);

        Task<BaseApiResponse> GetAll();

        Task<BaseApiResponse> CreateRole(string roleName);

        Task<BaseApiResponse> DeleteRole(string roleId);
    }
}
