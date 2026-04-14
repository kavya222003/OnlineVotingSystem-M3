using BCrypt.Net;
namespace OnlineVoting_API.Helpers
{
    public class PasswordHasher
    {
        public string Hash(string password)
    => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Verify(string password, string hash)
            => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
