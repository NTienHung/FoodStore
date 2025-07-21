using System.Net.Mail;
using System.Net;

namespace FoodStoreClient.Pages.Guest.Verify_Email
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string name)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var smtpClient = new SmtpClient(smtpSettings["SmtpServer"])
            {
                Port = int.Parse(smtpSettings["Port"]),
                Credentials = new NetworkCredential(smtpSettings["Username"], smtpSettings["Password"]),
                EnableSsl = bool.Parse(smtpSettings["UseSSL"]),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = $"Hi {name}",
                Body = "Thank you for registering an account at Restoran",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
