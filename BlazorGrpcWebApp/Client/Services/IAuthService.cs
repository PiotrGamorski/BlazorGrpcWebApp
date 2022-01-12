using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Services
{
    public interface IAuthService
    {
        Task<GenericAuthResponse<int>> Register(UserRegister userRegister);
        Task<GenericAuthResponse<string>> Login(UserLogin userLogin);
    }
}
