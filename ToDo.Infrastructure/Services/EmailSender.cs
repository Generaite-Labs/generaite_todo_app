using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using ToDo.Infrastructure.Configuration;
using ToDo.Domain.Entities;

namespace ToDo.Infrastructure.Services
{
    public class EmailSender : IEmailSender, Microsoft.AspNetCore.Identity.IEmailSender<ApplicationUser>
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailSender(IOptions<EmailConfiguration> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(_emailConfig.SmtpServer, _emailConfig.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailConfig.SmtpUsername, _emailConfig.SmtpPassword),
                EnableSsl = true
            };

            await client.SendMailAsync(
                new MailMessage(_emailConfig.SmtpUsername, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }

        public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            return SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            return SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        }

        public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            return SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        }
    }
}
