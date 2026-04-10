namespace Backend.Interfaces
{
    public interface IEmailService
    {
        public Task SendPasswordResetEmail(string email, string token);
    }
}
