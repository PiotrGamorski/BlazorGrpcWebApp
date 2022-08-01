using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface ILeaderboardService
    {
        IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; }

        Task GetLeaderboardWithRest();
        Task GetLeaderboardWithGrpc();
        Task<bool> ShowBattleLogsWithGrpc(int opponentId);
        Task<List<string>> GetBattleLogsWithGrpc(int opponentId);
    }
}
