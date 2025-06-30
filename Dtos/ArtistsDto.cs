using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Dtos
{
    [Keyless]
    public class ArtistDto
    {
        public string Bio { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
