using System.Collections.Generic;

namespace UserManagement_Backend.DTOs
{
    public class UserForListingDto : UserDto
    {
        public IList<string> Roles { get; set; }
    }
}
