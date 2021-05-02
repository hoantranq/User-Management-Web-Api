using Microsoft.EntityFrameworkCore;
using System;

namespace UserManagement_Backend.Models
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }

        public DateTime Expires { get; set; }

        public DateTime Created { get; set; }

        public DateTime? Revoked { get; set; }



        public bool IsExpired => DateTime.UtcNow >= Expires;

        public bool IsActive => Revoked == null && !IsExpired;
    }
}
