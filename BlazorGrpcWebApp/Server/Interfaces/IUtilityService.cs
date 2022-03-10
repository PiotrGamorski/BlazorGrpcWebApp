using BlazorGrpcWebApp.Shared.Entities;

namespace BlazorGrpcWebApp.Server.Interfaces
{
    public interface IUtilityService
    {
        int GetUserUserId();
        Task<User?> GetUser();
    }
}
