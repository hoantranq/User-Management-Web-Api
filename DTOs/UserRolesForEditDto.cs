using System.Collections.Generic;

namespace UserManagement_Backend.DTOs
{
    public class UserRolesForEditDto
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public IList<UserRolesDto> UserRoles { get; set; }
    }

    public class UserRolesDto
    {
        public string RoleName { get; set; }

        public bool Selected { get; set; }
    }
}
