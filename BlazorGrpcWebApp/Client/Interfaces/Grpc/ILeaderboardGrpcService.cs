using BlazorGrpcWebApp.Shared;

namespace BlazorGrpcWebApp.Client.Interfaces.Grpc
{
    public interface ILeaderboardGrpcService
    {
        IList<GrpcUserGetLeaderboardResponse> Leaderboard { get; set; }

        Task GetLeaderboardWithGrpc();
        Task<bool> ShowBattleLogsWithGrpc(int opponentId);
        Task<List<string>> GetBattleLogsWithGrpc(int opponentId);
    }
}
