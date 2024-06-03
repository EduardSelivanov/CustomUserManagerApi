using System.Security.Cryptography;
using System.Text;

namespace CustomUserManagerApi.Apllication_Layer.Services.PasswordHasher
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GenerateHashedPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                return Convert.ToBase64String(hashBytes);
            }

        }
    }
}
