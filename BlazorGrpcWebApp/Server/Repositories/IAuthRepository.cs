using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Server.Repositories
{
    public interface IAuthRepository
    {
        Task<GenericAuthResponse<int>> Register(User user, string password);
        Task<GenericAuthResponse<string>> Login(string emial, string password);
        Task<bool> UserExists(string email);
    }
}
