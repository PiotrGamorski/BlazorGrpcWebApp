using BlazorGrpcWebApp.Shared;
using BlazorGrpcWebApp.Shared.Models.UI_Models;

namespace BlazorGrpcWebApp.Client.Interfaces
{
    public interface ILeaderboardService
    {
        IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; }

        Task GetLeaderboardRestApi();
        Task DoGrpcGetLeaderboard();
        Task<bool> DoGrpcShowBattleLogs(int opponentId);
        Task<List<string>> DoGrpcGetBattleLogs(int opponentId);
    }
}
