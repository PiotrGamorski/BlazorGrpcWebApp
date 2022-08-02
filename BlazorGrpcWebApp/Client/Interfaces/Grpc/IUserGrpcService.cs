using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Grpc
{
    public interface IUserGrpcService
    {
        Task<GrpcUserExistsResponse> DoGrpcUserExists(GrpcUserExistsRequest request);
        Task<RegisterGrpcUserResponse> DoGrpcUserRegister(RegisterGrpcUserRequest request, int deadline);
        Task<RegisterGrpcUserResponse> RegisterWithGrpc(UserRegister request, int startUnitId, int deadline);
        Task<LoginGrpcUserRespone> DoGrpcUserLogin(LoginGrpcUserRequest request, int deadline);
        Task<GrpcUserBananasResponse> DoGrpcGetUserBananas(GrpcUserBananasRequest request);
        Task<GrpcUserAddBananasResponse> DoGrpcUserAddBananas(GrpcUserAddBananasRequest request);
    }
}
