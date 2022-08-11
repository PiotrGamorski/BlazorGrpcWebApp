using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces
{
    public interface IAuthService
    {
        Task<GenericAuthResponse<int>> Register(User user, string password);
        Task<GenericAuthResponse<string>> Login(string emial, string password);
        Task<bool> UserExists(string email);
    }
}
