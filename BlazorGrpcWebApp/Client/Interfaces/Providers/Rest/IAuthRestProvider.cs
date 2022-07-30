using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Providers.Rest
{
    public interface IAuthRestProvider
    {
        Task<GenericAuthResponse<string>?> Login(UserLogin userLogin);
        Task<GenericAuthResponse<int>?> Register(UserRegister userRegister);
    }
}
