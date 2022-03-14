using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcUserUnitService
    {
        Task<GrpcUserUnitResponse> DoGrpcBuildUserUnit(int unitId);
        Task<List<UserUnitResponse>> DoGrpcGetUserUnitAsync();
    }
}
