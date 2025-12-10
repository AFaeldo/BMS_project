using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BMS_project.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var port = int.Parse(emailSettings["Port"]);
            var senderEmail = emailSettings["SenderEmail"];
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];
            // SSL usage in MailKit depends on the port/server capability.
            // For Mailtrap port 587, we usually use StartTls.
            
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("BMS System", senderEmail));
            email.To.Add(new MailboxAddress("", toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = message
            };
            email.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                // Connect to the server
                // SecureSocketOptions.StartTls is typical for port 587
                await client.ConnectAsync(smtpServer, port, SecureSocketOptions.StartTls);

                // Authenticate
                await client.AuthenticateAsync(username, password);

                // Send
                await client.SendAsync(email);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}