using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Interfaces.Grpc
{
    public interface IArmyGrpcService
    {
        Task<GrpcReviveArmyResponse> DoGrpcReviveArmy();
        Task<GrpcHealUnitResponse> DoGrpcHealUnit(int userUnitId);
    }
}
