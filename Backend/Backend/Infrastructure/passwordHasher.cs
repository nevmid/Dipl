namespace Backend.Infrastructure
{
    public class PasswordHasher
    {
        public string Generate(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);

            return hashedPassword;
        }

        public bool Verify(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
        }
    }
}
