using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcUserUnitService
    {
        Task<GrpcUserUnitResponse> DoGrpcBuildUserUnit(int unitId);
        Task<List<UserUnitDto>> DoGrpcGetUserUnitAsync();
        Task<DeleteGrpcUserUnitResponse> DoDeleteUserUnitGrpc(int userUnitId);
    }
}
