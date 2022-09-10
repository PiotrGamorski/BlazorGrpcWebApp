using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Enums;
using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IArmyRestService
    {
        Task<List<UserUnitDto>> GetUserUnits();
        Task<HttpResponseMessage> HealUserUnit(int userUnitId);
        Task<HttpResponseMessage> DeleteUserUnit(int userUnitId);
        Task<HttpResponseMessage> ReviveUserUnits();
        Task<GenericAuthResponse<IList<UserLastActivityDto>>?> GetUserLastActivities(int userId, Page page, int activitiesNumber);
    }
}
