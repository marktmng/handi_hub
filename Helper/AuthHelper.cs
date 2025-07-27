using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers
{
    public class AuthHelper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthHelper(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("HandiHubConnection");
        }

        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltString = _config.GetSection("AppSettings:PasswordKey").Value +
                                        Convert.ToBase64String(passwordSalt);

            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            );
        }

        public string CreateToken(int userId)
        {
            var claims = new[]
            {
                new Claim("userId", userId.ToString())
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value ?? "";
            var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));
            var credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }

        public bool SetPassword(LoginDto userForPassword)
        {
            byte[] passwordSalt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetNonZeroBytes(passwordSalt);
            }

            byte[] passwordHash = GetPasswordHash(userForPassword.Password, passwordSalt);

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("HandiHub.spRegistration_Upsert", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 50)).Value = userForPassword.Email;
                command.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.VarBinary)).Value = passwordHash;
                command.Parameters.Add(new SqlParameter("@PasswordSalt", SqlDbType.VarBinary)).Value = passwordSalt;

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // public string CreateRefreshToken()
        // {
        //     var randomNumber = new byte[32];
        //     using (var rng = RandomNumberGenerator.Create())
        //     {
        //         rng.GetBytes(randomNumber);
        //     }
        //     return Convert.ToBase64String(randomNumber);
        // }

    }
}
