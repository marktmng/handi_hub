namespace DotnetAPI.Dtos
{
    public class UsersDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }

        public UsersDto() //  Constructor to initialize properties
        {
            // if any property is null, set it to an empty string

            FirstName = FirstName ?? string.Empty;
            LastName = LastName ?? string.Empty;
            UserName = UserName ?? string.Empty;
            Email = Email ?? string.Empty;
            PhoneNumber = PhoneNumber ?? string.Empty;
            Role = Role ?? string.Empty;
        }
    }
}
