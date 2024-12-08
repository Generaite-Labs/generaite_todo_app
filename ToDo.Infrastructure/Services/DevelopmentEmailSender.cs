using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using ToDo.Domain.Entities;
using System.Threading.Tasks;
using System.Web;

namespace ToDo.Infrastructure.Services
{
  public class DevelopmentEmailSender : IEmailSender, IEmailSender<ApplicationUser>
  {
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
      Console.WriteLine($"Development: Sending email to {email}");
      Console.WriteLine($"Subject: {subject}");
      Console.WriteLine($"Message: {htmlMessage}");
      return Task.CompletedTask;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
      // Decode the link to prevent double-encoding
      string decodedLink = HttpUtility.HtmlDecode(confirmationLink);
      return SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{decodedLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
      // Decode the link to prevent double-encoding
      Console.WriteLine("sendpassword");
      string decodedLink = HttpUtility.HtmlDecode(resetLink);
      return SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{decodedLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
      return SendEmailAsync(email, "Your password reset code", $"Your password reset code is: {resetCode}");
    }
  }
}
