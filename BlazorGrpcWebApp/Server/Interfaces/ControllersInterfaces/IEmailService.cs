using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailDto request);
    }
}
