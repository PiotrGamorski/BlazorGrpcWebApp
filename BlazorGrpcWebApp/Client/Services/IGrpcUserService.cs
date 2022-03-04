using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Services
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
