using System.Threading.Tasks;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.Auths
{
    public interface IAuthService
    {
        Task<BaseApiResponse> RegisterAsync(UserForRegisterDto userForRegisterDto);

        Task<BaseApiResponse> LoginAsync(UserForLoginDto userForLoginDto);

        Task<BaseApiResponse> RefreshTokenAsync(string refreshToken);

        Task<BaseApiResponse> RevokeTokenAsync(string refreshToken);

        Task<BaseApiResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto);

        Task<BaseApiResponse> ResetPasswordAsync(UserForResetPasswordDto userForResetPasswordDto);
    }
}
