using BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces;
using BlazorGrpcWebApp.Shared.Dtos;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using System.Text;

namespace BlazorGrpcWebApp.Server.Services.ControllersServices
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            var sb = new StringBuilder();
            sb.Append($"{request.Body}");
            email.Body = new TextPart(TextFormat.Html) { Text =  sb.ToString()};

            using (var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_configuration.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_configuration.GetSection("EmailUsername").Value, _configuration.GetSection("EmailPassword").Value);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
