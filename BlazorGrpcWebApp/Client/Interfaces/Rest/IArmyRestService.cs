using BlazorGrpcWebApp.Shared.Dtos;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IArmyRestService
    {
        Task<List<UserUnitDto>> GetUserUnits();
        Task<HttpResponseMessage> HealUserUnit(int userUnitId);
        Task<HttpResponseMessage> DeleteUserUnit(int userUnitId);
        Task<HttpResponseMessage> ReviveUserUnits();
    }
}
