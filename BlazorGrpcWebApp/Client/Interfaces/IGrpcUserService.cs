using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcUserService
    {
        Task<GrpcUserExistsResponse> DoGrpcUserExists(GrpcUserExistsRequest request);
        Task<RegisterGrpcUserResponse> DoGrpcUserRegister(RegisterGrpcUserRequest request, int deadline);
        Task<RegisterGrpcUserResponse> Register(UserRegister request, int deadline);
        Task<LoginGrpcUserRespone> DoGrpcUserLogin(LoginGrpcUserRequest request, int deadline);
        Task<GrpcUserBananasResponse> DoGrpcGetUserBananas(GrpcUserBananasRequest request);
        Task<GrpcUserAddBananasResponse> DoGrpcUserAddBananas(GrpcUserAddBananasRequest request);
    }
}
