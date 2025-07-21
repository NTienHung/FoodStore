using System.Net.Mail;
using System.Net;

namespace FoodStoreClient.Pages.Guest.Verify_Email
{
    public class ForgotPasswordService
    {
        private readonly IConfiguration _configuration;

        public ForgotPasswordService(IConfiguration configuration)
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

            var verificationUrl = $"http://localhost:5128/Guest/Restoran_Forgot_Password/Change_Password?email={toEmail}";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSettings["SenderEmail"], smtpSettings["SenderName"]),
                Subject = $"Verify your email! Your username: {name}",
                Body = $"Please verify your account by clicking this link: {verificationUrl}",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
