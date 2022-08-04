using BlazorGrpcWebApp.Shared.Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface IBananaService
    {
        event Action OnChange;
        int Bananas { get; set; }
        Task BananasChanged();
        Task EatBananas(int amount);
        Task AddBananas (int amount);
        Task GrpcAddBananas(int amount);
        Task<GenericAuthResponse<int>> GetBananas(int authUserId);
        Task GrpcGetBananas();
    }
}
