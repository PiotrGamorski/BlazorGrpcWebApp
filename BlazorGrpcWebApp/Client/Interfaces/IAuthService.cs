using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IAuthService
    {
        Task<GenericAuthResponse<int>> Register(UserRegister userRegister);
        Task<GenericAuthResponse<string>> Login(UserLogin userLogin);
    }
}
