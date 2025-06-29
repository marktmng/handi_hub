using System.Text.Json.Serialization;

namespace DotnetAPI.Models
{
    public class Customer
    {
        public int? CustomerId { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } // Navigation property to User
    }

}
