using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IAuthService
    {
        Task<GenericAuthResponse<int>> Register(UserRegister userRegister);
        Task<GenericAuthResponse<string>> Login(UserLogin userLogin);
    }
}
