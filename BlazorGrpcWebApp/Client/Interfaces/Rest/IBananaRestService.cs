using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces.Rest
{
    public interface IBananaRestService
    {
        Task BananasChanged();
        Task EatBananas(int amount);
        Task<GenericAuthResponse<int>> GetBananas(int authUserId);
        Task AddBananas(int amount);
    }
}
