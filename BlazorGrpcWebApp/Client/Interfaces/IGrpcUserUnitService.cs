using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IGrpcUserUnitService
    {
        Task<GrpcUserUnitResponse> DoGrpcBuildUserUnit(int unitId);
    }
}
