using BlazorGrpcWebApp.Shared.Dtos;
using BlazorGrpcWebApp.Shared.Enums;

namespace BlazorGrpcWebApp.Server.Interfaces.ControllersInterfaces
{
    public interface IUserLastActivityService
    {
        Task<IList<UserLastActivityDto>?> GetUserLastActivites(int userId, Page page, int activitiesNumber);
    }
}
