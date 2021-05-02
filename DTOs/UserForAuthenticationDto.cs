using System;
using System.Collections.Generic;

namespace UserManagement_Backend.DTOs
{
    public class UserForAuthenticationDto : UserDto
    {
        public string AccessToken { get; set; }

        public IEnumerable<string> Roles { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }
    }
}
