﻿using System.Threading.Tasks;
using UserManagement_Backend.DTOs;
using UserManagement_Backend.Models.Responses;

namespace UserManagement_Backend.Services.Auths
{
    public interface IAuthService
    {
        Task<BaseApiResponse> RegisterAsync(UserForRegisterDto userForRegisterDto);
    }
}
