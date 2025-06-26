using System.Text.Json.Serialization;

namespace DotnetAPI.Models
{
    public class Artist
    {
        public int? ArtistId { get; set; }
        public int UserId { get; set; }

        // From Users table
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // From Artists table
        public string Bio { get; set; }
    }

}
