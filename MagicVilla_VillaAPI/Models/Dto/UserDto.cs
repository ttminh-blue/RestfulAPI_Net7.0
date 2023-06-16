using Microsoft.AspNetCore.Identity;

namespace MagicVilla_VillaAPI.Models.Dto
{
    public class UserDto
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public List<IdentityError> Errors { get; set; }
    }
}
