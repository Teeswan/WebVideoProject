using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // TODO: Replace with real email sending (SendGrid, SMTP, etc.)
            Console.WriteLine($"EMAIL TO: {email}");
            Console.WriteLine($"SUBJECT: {subject}");
            Console.WriteLine($"MESSAGE: {htmlMessage}");

            return Task.CompletedTask;
        }
    }
}
