using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UserManagement_Backend.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
