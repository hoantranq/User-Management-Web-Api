using System.Collections.Generic;

namespace UserManagement_Backend.DTOs
{
    public class RoleForListingUserDto
    {
        public string Id { get; set; }

        public string RoleName { get; set; }

        public List<UserDto> Users { get; set; }
    }
}
