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
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using UserManagement_Backend.Services.Emails;

namespace UserManagement_Backend.Services.Auths
{
    public class AuthService : IAuthService
    {
        #region Private Fields
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public AuthService(
            UserManager<User> userManager,
            IOptions<JWT> jwt,
            IMapper mapper,
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _jwt = jwt.Value;
            _configuration = configuration;
            _emailService = emailService;
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
                return BaseApiResponseHelper.GenerateApiResponse(false, "register new user", null, new List<string> { $"Email {newUser.Email} is already registered." });
            }

            var result = await _userManager.CreateAsync(newUser, userForRegisterDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, Authorization.DEFAULT_ROLE.ToString());
            }

            return BaseApiResponseHelper.GenerateApiResponse(true, "Register new user", null, null);
        }

        // Login
        public async Task<BaseApiResponse> LoginAsync(UserForLoginDto userForLoginDto)
        {
            if (userForLoginDto == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "login", null, new List<string> { "Email or password is null." });
            }

            var user = await _userManager.FindByEmailAsync(userForLoginDto.Email);

            if (user == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "login", null, new List<string> { "Could not find any accounts registered with this email." });
            }

            // Checking user password
            if (await _userManager.CheckPasswordAsync(user, userForLoginDto.Password))
            {
                JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);

                var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

                var refreshToken = string.Empty;

                var refreshTokenExpiration = new DateTime();

                // Check if user has any active refreshToken then give them this refreshToken
                if (user.RefreshTokens.Any(token => token.IsActive))
                {
                    var activeRefreshToken = user.RefreshTokens.Where(token => token.IsActive == true).FirstOrDefault();

                    refreshToken = activeRefreshToken.Token;

                    refreshTokenExpiration = activeRefreshToken.Expires;
                }
                else // Otherwise create new refreshToken then update it to the database
                {
                    var newRefreshToken = CreateRefreshToken();

                    user.RefreshTokens.Add(newRefreshToken);

                    refreshToken = newRefreshToken.Token;

                    refreshTokenExpiration = newRefreshToken.Expires;

                    _context.Update(user);
                    _context.SaveChanges();
                }

                var userForAuthenticationDto = new UserForAuthenticationDto()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                    RefreshToken = refreshToken,
                    RefreshTokenExpiration = refreshTokenExpiration
                };

                return BaseApiResponseHelper.GenerateApiResponse(true, "Logged in", new List<UserForAuthenticationDto> { userForAuthenticationDto }, null);
            }

            return BaseApiResponseHelper.GenerateApiResponse(false, "login", null, new List<string> { "Username or password is incorrect." });
        }

        // Refresh Token
        public async Task<BaseApiResponse> RefreshTokenAsync(string refreshToken)
        {
            // STEP 1: Check if refreshToken match any users
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get refresh token", null, new List<string> { "Refresh token did not match any users." });
            }

            // STEP 2: Check if refreshToken is active or not
            var refreshTokenFromDb = user.RefreshTokens.Single(x => x.Token == refreshToken);

            if (!refreshTokenFromDb.IsActive)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get refresh token", null, new List<string> { "Refresh token is not active." });
            }

            // STEP 3: Revoke the current refresh token
            refreshTokenFromDb.Revoked = DateTime.UtcNow;

            // STEP 4: Generate a new refresh token and save to database
            var newRefreshToken = CreateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);

            _context.Update(user);

            await _context.SaveChangesAsync();

            // FINALLY: Generate new JWT Token
            JwtSecurityToken jwtSecurityToken = await CreateJwtToken(user);

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);

            var userForAuthenticationDto = new UserForAuthenticationDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Email = user.Email,
                Roles = roles,
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.Expires
            };

            return BaseApiResponseHelper.GenerateApiResponse(true, "Get refresh token", new List<UserForAuthenticationDto> { userForAuthenticationDto }, null);
        }

        // Revoke Token
        public async Task<BaseApiResponse> RevokeTokenAsync(string refreshToken)
        {
            // STEP 1: Check if refreshToken match any users
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get refresh token", null, new List<string> { "Refresh token did not match any users." });
            }

            // STEP 2: Check if refreshToken is active or not
            var refreshTokenFromDb = user.RefreshTokens.Single(x => x.Token == refreshToken);

            if (!refreshTokenFromDb.IsActive)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "get refresh token", null, new List<string> { "Refresh token is not active." });
            }

            // STEP 3: Revoke the current refresh token
            refreshTokenFromDb.Revoked = DateTime.UtcNow;

            // STEP 4: Delete all revoked tokens
            var revokedTokens = user.RefreshTokens.Where(t => t.IsActive == false).ToList();

            _context.RemoveRange(revokedTokens);
            _context.Update(user);

            await _context.SaveChangesAsync();

            return BaseApiResponseHelper.GenerateApiResponse(true, "Revoke a token", null, null);
        }

        // Forgot Password
        public async Task<BaseApiResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if (forgotPasswordDto == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { "Email is null." });
                }

                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

                if (user == null)
                {
                    return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { "Could not found any users registered with this email." });
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = Encoding.UTF8.GetBytes(token);

                var validToken = WebEncoders.Base64UrlEncode(encodedToken);

                string url = $"{_configuration["ApplicationUrl"]}/reset-password?email={forgotPasswordDto.Email}&token={validToken}";

                var mailRequest = new MailRequest
                {
                    ToEmail = forgotPasswordDto.Email,
                    Subject = "Reset Password Confirmation",
                    Attachments = null,
                    Body = "<h1>Follow the instructions to reset your password</h1>" +
                            $"<p>To reset your password <a href='{url}'>Click here</a></p>"
                };

                await _emailService.SendEmailAsync(mailRequest);

                return BaseApiResponseHelper.GenerateApiResponse(true, "Reset password url has been sent to the email", null, null);
            }
            catch (Exception ex)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { $"{ex.Message}" });
            }
        }

        public async Task<BaseApiResponse> ResetPasswordAsync(UserForResetPasswordDto userForResetPasswordDto)
        {
            if (userForResetPasswordDto == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { "Invalid request." });
            }

            var user = await _userManager.FindByEmailAsync(userForResetPasswordDto.Email);

            if (user == null)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { "Could not found any users registered with this email." });
            }

            var decodedToken = WebEncoders.Base64UrlDecode(userForResetPasswordDto.Token);

            string normalToken = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, normalToken, userForResetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                return BaseApiResponseHelper.GenerateApiResponse(false, "handle your request.", null, new List<string> { "Could not reset password." });
            }

            return BaseApiResponseHelper.GenerateApiResponse(true, "Reset password", null, null);
        }
        #endregion

        #region Private Methods
        private async Task<JwtSecurityToken> CreateJwtToken(User user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshToken CreateRefreshToken()
        {
            var randomNumber = new byte[32];

            using (var generator = new RNGCryptoServiceProvider())
            {
                generator.GetBytes(randomNumber);

                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Created = DateTime.UtcNow
                };
            }
        }
        #endregion
    }
}
