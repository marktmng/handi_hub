namespace DotnetAPI.Dtos
{
    public class LoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public LoginConfirmationDto()
        {
            PasswordHash = PasswordHash ?? new byte[0];
            PasswordSalt = PasswordSalt ?? new byte[0];
        }
    }
}