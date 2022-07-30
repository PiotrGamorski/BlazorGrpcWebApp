using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IAuthRestService
    {
        Task<GenericAuthResponse<string>?> Login(UserLogin userLogin);
        Task<GenericAuthResponse<int>?> Register(UserRegister userRegister);
    }
}
