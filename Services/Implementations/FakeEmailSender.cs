using Microsoft.AspNetCore.Identity.UI.Services;
namespace Youtube_Entertainment_Project.Services.Implementations
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"EMAIL SENT (FAKE): {email} | {subject}");
            return Task.CompletedTask;
        }
    }
}
