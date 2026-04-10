using System.Net;
using System.Net.Mail;
using Backend.Interfaces;

namespace Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(configuration["EmailSettings:SmtpPort"]!),
                Credentials = new NetworkCredential(
                _configuration["EmailSettings:SmtpUsername"],
                _configuration["EmailSettings:SmtpPassword"]),
                EnableSsl = true
            };
        }

        public async Task SendPasswordResetEmail(string email, string token)
        {
            try
            {
                var baseUrl = "http://localhost";
                var resetLink = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";

                var subject = "Восстановление пароля";
                var body = $@"
                <h2>Восстановление пароля</h2>
                <p>Здравствуйте!</p>
                <p>Для восстановления пароля перейдите по ссылке:</p>
                <p><a href='{resetLink}'>Восстановить пароль</a></p>
                <p>Ссылка действительна в течение 1 часа.</p>
                <p>Если вы не запрашивали восстановление пароля, просто проигнорируйте это письмо.</p>
                <br>
                <p>С уважением,<br>Команда кинотеатра</p>";

                await SendEmail(email, subject, body);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task SendEmail(string to, string subject, string body)
        {
            try
            {
                var from = _configuration["EmailSettings:FromEmail"];

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(from),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException smtpEx)
            {
                throw new InvalidOperationException($"Ошибка отправки email: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
