using System.Security.Claims;
using System.Threading.Tasks;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.Users
{
    public interface IUserService
    {
        Task<BaseApiResponse> GetAll(ClaimsPrincipal claimsPrincipal);

        Task<BaseApiResponse> GetUserById(string userId);
    }
}
