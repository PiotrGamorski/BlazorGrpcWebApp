using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Models;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IAuthRestService
    {
        Task<GenericAuthResponse<string>?> Login(UserLogin userLogin);
        Task<GenericAuthResponse<int>?> Register(UserRegisterRequestDto userRegister);
        Task<GenericAuthResponse<object>> Verify(VerifyCodeRequestDto request);
    }
}
