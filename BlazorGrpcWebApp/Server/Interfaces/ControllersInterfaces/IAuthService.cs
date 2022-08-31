using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Entities;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces
{
    public interface IAuthService
    {
        Task<GenericAuthResponse<int>> Register(User user, string password, int startUnitId);
        Task<GenericAuthResponse<string>> Login(string emial, string password);
        Task<GenericAuthResponse<bool>> Verify(VerifyCodeRequestDto request);
        Task<bool> UserEmailExists(string email);
        Task<bool> UserNameExists(string userName);
    }
}
